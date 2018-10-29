using System;
using FactomSharp.Factomd;
using FactomSharp.Factomd.API;

namespace FactomSharp.Helper
{
    public class EntryItem 
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
    
    
        public event EventHandler Complete;
        public event EventHandler<State> StatusChange;
    
        Action<EntryItem>                   Process;
        public string                       TxId        {get; set;}
        public string                       EntryHash   {get; set;}
        
        public Helper.Chain.BlockingMode    Blocking    {get; set;}
        public Helper.Chain                 Chain       {get; set;}
        
        public Factomd.API.CommitEntry      Commit      {get; set;}
        public Factomd.API.RevealEntry      Reveal      {get; set;}
        public APIError                     ApiError    {get; set;}
        
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
					StatusChange?.Invoke(this, _state);
					Chain.SetQueueItemStatusChange(this,_state);
                    if (_state == State.DBlockConfirmed) Complete?.Invoke(this,null);
                }
            }
        }
    
        public EntryItem(Helper.Chain chain, Action<EntryItem> action, Helper.Chain.BlockingMode blocking = Chain.BlockingMode.Request)
        {
            Process = action;
            Blocking = blocking;
            Chain = chain;
        }
        
        public void Run()
        {
            try
            {
                var lastStatus = Status;
                
                if (Status == State.Queued)
                {
                    Process.Invoke(this);
                }
                else if (Status >= State.RevealOK || Status < State.DBlockConfirmed)
                {
                   var ack = new Ack(Chain.FactomD);
                   switch (ack.CheckReveal(Chain.ChainID,EntryHash))
                   {
                    case Ack.Status.DBlockConfirmed:
                        Status = State.DBlockConfirmed;
                        break;
                    case Ack.Status.TransactionACK:
                        Status = State.TransactionACK;
                        break;
                    case Ack.Status.NotConfirmed:
                    case Ack.Status.RequestFailed:
                    case Ack.Status.Unknown:
                        break;
                   }
                }
                else
                {
                    //retry?
                }
            }catch (Exception ex)
            {
                ApiError = new APIError(ex);
                Status = State.Exception;
            }
        }
    }
}