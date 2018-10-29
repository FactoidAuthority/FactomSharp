using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FactomSharp.Factomd;
using FactomSharp.Factomd.API;
using Newtonsoft.Json;

namespace FactomSharp.Helper
{
    public class Chain
    {
        public enum State
        {
            Exception           = -100,
            CommitFail          = -10,
            RevealFail          = -20,
            Queued              = 0,
            CommitOK            = 10,
            RevealOK            = 20,
            TransactionACK      = 30,
            DBlockConfirmed     = 40,
            Complete            = 50,
            CompleteConfirmed   = 60
        }
        
        public enum BlockingMode
        {
            Request             = 1,
            TransactionACK      = 2,
            DBlockConfirmed     = 3
        }
        
        List<EntryItem> EntryQueue = new List<EntryItem>();
        
        Task TaskQueue = null;
        
        public FactomdRestClient    FactomD     {get; private set;}
        public ECAddress            EcAddress   {get; private set;}
        public string               ChainID     {get; private set;}
        
        
        
        public string                       TxId        {get; set;}
        public string                       EntryHash   {get; set;}
        public Factomd.API.CommitChain      Commit      {get; set;}
        public Factomd.API.RevealChain      Reveal      {get; set;}
        
        
        public int                  PollACKms   {get; set;} = 5000;
        public int                  PollDBACKms {get; set;} = 30000;
        
        public BlockingMode         Blocking {get; set;} = Chain.BlockingMode.Request;
        

		internal void SetQueueItemStatusChange(EntryItem queueItemEntry, EntryItem.State state)
		{
			QueueItemStatusChange?.Invoke(queueItemEntry, state);
		}

		AutoResetEvent queueBlock = new AutoResetEvent(false);
        readonly object queueLock = new object();
        
        public event EventHandler QueueStalled;
        public event EventHandler<EntryItem.State> QueueItemStatusChange;
        public event EventHandler<State> ChainStatusChange;
        
        State _state = State.Queued;
        public State Status       
        {   get
            {
                return _state;
            }
         
            set
            {
                if (_state != value)
                {
                    _state = value;
                    ChainStatusChange(this,_state);
                  //  if (_state == State.DBlockConfirmed) Complete(this,null);
                }
            }
        }
    
        public Chain(ECAddress ecAddress, String existingChainID = null)
        {
            EcAddress = ecAddress;
            FactomD = ecAddress.FactomD;
            ChainID = existingChainID;           
        }
        
        public int QueueCount { get { return EntryQueue.Count; } }
        public EntryItem[] QueueList { get { lock(queueLock){return EntryQueue.ToArray(); } } }
        
        public async Task<bool> Create(byte[] data, byte[][] extIDs = null, string chainIdString = null)
        {
            var task = Task.Run(() => 
            {
                try
                {
                    Commit = new CommitChain(FactomD);
                    var commitStatus = Commit.Run(data,EcAddress.Public,EcAddress.Secret,extIDs,chainIdString);
                   
                    if (commitStatus)
                    {
                        Status = State.CommitOK;
                        TxId = Commit?.Result?.result?.Txid ?? null;
                        Reveal = new RevealChain(FactomD);
                        if (Reveal.Run(Commit.Entry))
                        {
                            Status = State.RevealOK;
                            EntryHash = Reveal?.Result?.result?.Entryhash;
                            ChainID = Reveal?.Result?.result?.Chainid;
                            
                            var sleep = PollACKms;
                            var timeout = DateTime.UtcNow.AddMinutes(10);
                            do
                            {
                                Thread.Sleep(sleep);
                            
                               var ack = new Ack(FactomD);
                               switch (ack.CheckReveal(ChainID,EntryHash))
                               {
                                case Ack.Status.DBlockConfirmed:
                                    Status = State.DBlockConfirmed;
                                    return;
                                case Ack.Status.TransactionACK:
                                    Status = State.TransactionACK;
                                    sleep = PollDBACKms;
                                    if (Blocking != BlockingMode.DBlockConfirmed) return;
                                    break;
                                case Ack.Status.NotConfirmed:
                                case Ack.Status.RequestFailed:
                                case Ack.Status.Unknown:
                                    break;
                               }
                            } while (timeout < DateTime.UtcNow);
                        }
                        else
                        {
                            Status = State.RevealFail;
                        }
                    }
                    else
                    {
                        Status = State.CommitFail;
                    }
                }
                catch (Exception ex)
                {
                    var error = new APIError(ex);
                    Status = State.Exception;
                }
            });
            
            return false;
            
        }

        public EntryItem AddEntry(byte[] dataEntry, byte [][] ExtIDs = null)
        {

            Action<EntryItem> action = (process) => {
                try
                {
                    process.Commit = new CommitEntry(FactomD);
                    var commitStatus = process.Commit.Run(ChainID,dataEntry,EcAddress.Public,EcAddress.Secret,ExtIDs);
                   
                    if (commitStatus)
                    {
                        process.Status = EntryItem.State.CommitOK;
                        process.TxId = process.Commit?.Result?.result?.Txid ?? null;
                        process.Reveal = new RevealEntry(FactomD);
                        if (process.Reveal.Run(process.Commit.Entry))
                        {
                            process.Status = EntryItem.State.RevealOK;
                            process.EntryHash = process.Reveal?.Result?.result?.Entryhash;
                            process.Chain.ChainID = process.Reveal?.Result?.result?.Chainid;
                        }
                        else
                        {
                            var error = JsonConvert.DeserializeObject<APIError>(process.Reveal.JsonReply);
                            process.ApiError = error;
                            process.Status =  EntryItem.State.RevealFail;
                        }
                    }
                    else
                    {
                        var error = JsonConvert.DeserializeObject<APIError>(process.Commit.JsonReply);
                        process.ApiError = error;
                        process.Status = EntryItem.State.CommitFail;
                    }
                }
                catch (Exception ex)
                {
                    var error = new APIError(ex);
                    process.Status = EntryItem.State.Exception;
                }
            };
            
            
            var queueItem = new EntryItem(this,action,Blocking);
            lock(queueLock)
            {
                EntryQueue.Add(queueItem);
            }
            RunQueue();
            return queueItem;
        }

        public IEnumerable<EntryData> GetEntries()
        {
            //First we need the chain head.     
            var chainhead = new ChainHead(FactomD);
            chainhead.Run(ChainID);
            
            //Now we can get the last entry block
            var eblock = new EntryBlock(FactomD);
            eblock.Run(chainhead.Result.result.chainHead);
            
            while (true)
            {
                //iterate each entry hash
                foreach (var entryitem in eblock.Result.result.entrylist)
                {
                    //Get the entry, and yield
                    var entry = new Entry(FactomD);
                    entry.Run(entryitem.Entryhash);
                    yield return entry.Result.result.EntryData;
                }
                
                //Is there a previous entry/block to read?
                if (String.IsNullOrEmpty(eblock.Result.result.header.Prevkeymr)) break;
                //Request block
                eblock.Run(eblock.Result.result.header.Prevkeymr);
            }
        }
        
        void RunQueue()
        {
            EntryItem[] tempQueueList;
            if (TaskQueue == null || TaskQueue.Status != TaskStatus.Running)
            {
               TaskQueue = new TaskFactory().StartNew(() =>
               {
                  do
                  {
                     if (!String.IsNullOrEmpty(ChainID))
                     {
                         lock(queueLock)
                         {
                            tempQueueList = EntryQueue.ToArray();
                         }
                      
                         foreach (var item in tempQueueList)
                         {
                            item.Run();
                            
                            if (item.Status < EntryItem.State.Queued)
                            {
                               break;
                            }
                            else if (item.Status < EntryItem.State.TransactionACK)
                            {
                                if (item.Blocking >= BlockingMode.TransactionACK ) break;
                            }
                            else if (item.Status < EntryItem.State.DBlockConfirmed)
                            {
                                if (item.Blocking == BlockingMode.DBlockConfirmed ) break;
                            }
                            else
                            {
                                lock(queueLock)
                                {
                                    EntryQueue.Remove(item);
                                }
                            }
                         } 
                     }
                     if (EntryQueue.Count == 0) break;
                     queueBlock.WaitOne(5000);
                  } while(EntryQueue.Count>0);
                  
               });
            }
            else
            {
                queueBlock.Set();
            }
        }
    }
    
}
