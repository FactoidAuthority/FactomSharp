# FactomSharp .NET Factom node API DLL
### A .NET implementation of the Factom API written in C#.

##### Supports:

* factomd API V2
* factom-walletd API V2

#### Features:

* Provides all API functions.  Native Json-like classes & helper methods.
* Multi-node support, for automatic fallback.
* Asyncronious Helper classes:
    * Chain class
        * Open/Create chain
        * Read/Write entries
        * IEnumerable entry lists
        * Track commit status, with callbacks.
    * FCT/EC class.
        * Track status, with callbacks. (TODO)
        * Track Balances (TODO)
* HTTP, HTTPS, Proxy support (uses RestSharp)
* NuGet for easy installation

##### Depends on:

* Newtonsoft.json
* RestSharp
* dlech.Chaos.NaCL

---

##### To create a chain, and add some entries:

    var factomd = new FactomdRestClient("https://api.mynode.com:8088"); 
    var myEcAddress = "EC3PV61FYYEQFKwwCVEZj9m5Ge9TrPhE9A2N7pA9YPNM7PXNfuCh";
    var myEsAddress = "Es4Mn6eW8AGtNhH1YSGepbBofFzfJnq3RLFtCZ93kZ14VwGaeQHq";
    
    var address = new ECAddress(factomd,myEcAddress,myEsAddress);
    using (var chain = new Chain(address))
    {
      chain.ChainStatusChange += (o,a) => {
        Console.WriteLine($"ChainStatusChange: {a.ToString()}");
      };
            
      chain.QueueItemStatusChange += (o,a) => {
        var item = (EntryItem)o;
        Console.WriteLine($"EntryStatusChange {a.ToString()} {item?.ApiError?.error}");
      };
 
      chain.Create(Encoding.ASCII.GetBytes("This is my new chain"));

      for (int i =1; i< 10;i++)
      {
          var text = $"This is a test - hello! {i}";
          Console.WriteLine($"Writing: {text}");
          var entry = chain.AddEntry(Encoding.ASCII.GetBytes(text));
      }
    
      Console.ReadLine(); //Pause, let the chain thread run.
    }  

The chain class will monitor the entries.  The two status's of interest are:

    TransactionACK   - Found in network, but not written to the blockchain yet.
    DBlockConfirmed  - Found in Blockchain.

##### To read through the chain data:

    var factomd = new FactomdRestClient("https://api.mynode.com:8088"); 
    var myEcAddress = "EC3PV61FYYEQFKwwCVEZj9m5Ge9TrPhE9A2N7pA9YPNM7PXNfuCh";
    
    var address = new ECAddress(factomd,myEcAddress);
    using (var chain = new Chain(address,"3a6c770d8b152dcc80fa0a54fa931c6208e96f14f76dd616e51502a58836e9f8"))
    {
       foreach (var item in chain.GetEntries())
       {
           WriteLine(Encoding.ASCII.GetString(item.Content));
       }
    }

---
    
##### Full API support.
    
You will find all the API's in the namespace **FactomSharp.Factomd.API** and **FactomSharp.FactomWalletd.API**
example:

    var rate = new FactomSharp.Factomd.API.EntryCreditRate(factomd);
    rate.Run();
    WriteLine($"EC Rate {rate.Result.result.Rate}");
    
    
##### Multiple nodes:
Use a comma separated list of URLs:

    https://api.mynode1.com:8088;https://api.mynode2.com:8088;https://api.mynode3.com:8088;

If a node fails to reply, or timeouts, the next node in the list is selected.
