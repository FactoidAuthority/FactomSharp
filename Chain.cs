using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FactomSharp.Factomd;
using FactomSharp.Factomd.API;
using Newtonsoft.Json;

namespace FactomSharp
{
    /// <summary>
    /// Helper class, which manages chain operations.
    /// </summary>
    public class Chain : IDisposable
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
        bool stopQueue = false;        

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
    
        /// <summary>
        /// Open a new or existing chain.
        /// </summary>
        /// <param name="ecAddress">Ec address.</param>
        /// <param name="existingChainID">Existing chain identifier.</param>
        public Chain(ECAddress ecAddress, String existingChainID = null)
        {
            EcAddress = ecAddress;
            FactomD = ecAddress.FactomD;
            ChainID = existingChainID;           
        }
        
        /// <summary>
        /// Open an existing chain (Read only, without EC Address)
        /// </summary>
        /// <param name="factomClient">Factom client.</param>
        /// <param name="existingChainID">Existing chain identifier.</param>
        public Chain(FactomdRestClient factomClient, String existingChainID)
        {
            FactomD = factomClient;
            ChainID = existingChainID;           
        }
        
        
        public int QueueCount { get { return EntryQueue.Count; } }
        public EntryItem[] QueueList { get { lock(queueLock){return EntryQueue.ToArray(); } } }
        public void QueueStop()
        {
           stopQueue = true;
        }
        
        
        /// <summary>
        /// Create a new chain, with the first entry.
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="data">Data.</param>
        /// <param name="extIDs">Ext identifier.</param>
        /// <param name="chainIdString">Optional ChainID (a random ID will be created if not supplied).</param>
        
        public async Task<bool> Create(byte[] data, byte[][] extIDs = null, string chainIdString = null)
        {
        
            if (EcAddress == null) throw new Exception("No EcAddress provided - read only");
        
            var task = Task.Run(() => 
            {
                try
                {
                    Commit = new CommitChain(FactomD);
                    var commitStatus = Commit.Run(data,EcAddress,extIDs,chainIdString);
                   
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
                                    return;  //Complete, so quit
                                case Ack.Status.TransactionACK:
                                    Status = State.TransactionACK;
                                    sleep = PollDBACKms;
                                    if (Blocking != BlockingMode.DBlockConfirmed) return; //Complete if we are not looking for DBlockConfirmed
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
            if (EcAddress == null) throw new Exception("No EcAddress provided - read only");

            Action<EntryItem> action = (process) => {
                try
                {
                    process.Commit = new CommitEntry(FactomD);
                    var commitStatus = process.Commit.Run(ChainID,dataEntry,EcAddress,ExtIDs);
                   
                    if (commitStatus) //commit success?
                    {
                        process.Status = EntryItem.State.CommitOK;
                        process.TxId = process.Commit?.Result?.result?.Txid ?? null;
                        process.Reveal = new RevealEntry(FactomD);
                        if (process.Reveal.Run(process.Commit.Entry))
                        {
                            process.Status = EntryItem.State.RevealOK;
                            process.EntryHash = process.Reveal?.Result?.result?.Entryhash;
                        }
                        else  //Reveal failed
                        {
                            var error = JsonConvert.DeserializeObject<APIError>(process.Reveal.JsonReply);
                            process.ApiError = error;
                            process.Status =  EntryItem.State.RevealFail;
                        }
                    }
                    else //Commit failed 
                    {
                        var error = JsonConvert.DeserializeObject<APIError>(process.Commit.JsonReply);
                        process.ApiError = error;
                        process.Status = EntryItem.State.CommitFail;
                    }
                }
                catch (Exception ex)
                {
                    var error = new APIError(ex);
                    process.ApiError = error;
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
            stopQueue = false;
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
                      
                         foreach (var item in tempQueueList)  //iterate though queue
                         {
                            item.Run();
                            
                            if (item.Status < EntryItem.State.Queued) //Anything less than Queued, is an error state
                            {
                               break;  //Quit queue
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
                  } while(EntryQueue.Count>0 && !stopQueue);
               });
            }
            else
            {
                queueBlock.Set();
            }
        }
        
        public void Dispose()
        {
            EntryQueue.Clear();
            queueBlock.Set();
            if (TaskQueue!=null)
            {
                if (TaskQueue.Status == TaskStatus.Running) TaskQueue.Wait(100);
                TaskQueue.Dispose();
            }
        }        
    }
    
}
