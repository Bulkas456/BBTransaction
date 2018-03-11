If you need a simple and customizable transaction mechanism for a code methods which is able to continue processing steps after an unexpected crash or a power lost then this library is for you!
# Availability and Requirements:
The code is available for:
* .NET Framework 3.5 (no async await functionality)
* .NET Framework 4.5
* NET Core 1.0
# Build notes
You can remove the async await functionality from builds by specifying 'NOASYNC' in compilation symbols. 
# How it works
The library produces a transaction object which runs actions for a steps in a specific order. When an exception occurred or a transaction is cancelled then undo methods for a finished steps are processed. When transaction finish successfully then post actions are invoked.
Let's see an example:
First of all you need an idea for a transaction, i.e. safely write a binary data to a file:
1. Create a simple DTO for all necessary data to a file write operation:
```c#
public class FileWriteData
{
    public byte[] DataToWrite { get; set; }
    public string File { get; set; }
}
```
2. Create a transaction step id, it can be any type: enum, string, int etc.:
```c#
public enum WriteStepId
{
    CreateBackup,
    WriteData
}
```
3. Create a transaction instance with an appropriable settings using a factory:
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                       .Create<WriteStepId, FileWriteData>(options => 
{
    options.TransactionInfo.Name = "Example transaction";
});
```
4. Add steps to the created transaction:
```c#
transaction.Add(new TransactionStep<WriteStepId, FileWriteData>()
{
    Id = WriteStepId.CreateBackup,
    AsyncStepAction = CreateBackup,
    AsyncPostAction = CreateBackupPost
});
transaction.Add(new TransactionStep<WriteStepId, FileWriteData>()
{
    Id = WriteStepId.WriteData,
    AsyncStepAction = WriteData,
    AsyncUndoAction = WriteDataUndo
});

...

private async Task CreateBackup(FileWriteData data, IStepTransactionSessionInfo<WriteStepId> info)
{
    // Create a backup for an original file.
}

private async Task CreateBackupPost(FileWriteData data, IPostTransactionSessionInfo<WriteStepId> info)
{
    // Remove the created backup.
}

private async Task WriteData(FileWriteData data, IStepTransactionSessionInfo<WriteStepId> info)
{
    // Write the data from the 'data' instance to a specific file.
}

private async Task WriteDataUndo(FileWriteData data, IUndoTransactionSessionInfo<WriteStepId> info)
{
    // Remove the file and copy the file backup.
}
```
5. Run the transaction with data:
```c#
ITransactionResult<FileWriteData> result = await transaction.Run(settings => 
{
    settings.Mode = RunMode.Run;
    settings.Data = new FileWriteData()
    {
        DataToWrite = new byte[] { 0x01, 0x02 },
        File = "path to the file"
    };
});
```
6. Check the result:
```c#
switch (result.Result)
{
    case ResultType.Success:
        break;
    case ResultType.Cancelled:
        break;
    case ResultType.Failed:
        break;
    case ResultType.NoTransactionToRecover:
        break;
}
```
# Transaction object settings and features:
 ## Settings for a transaction creation
* Transaction logs forwarding: if an additional logs for a transaction are needed then you can specify a transaction log forwarding: 
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()              
                                                       .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.Name = "Example transaction";
    options.LoggerContext.DebugFormatAction = (format, parameters) =>
    {
        Debug.WriteLine(format, parameters);
    };
    options.LoggerContext.ErrorFormatAction = (format, parameters) =>
    {
        Trace.WriteLine(string.Format(format, parameters));
    };
    options.LoggerContext.ExecutionTimeLogAction = (time, format, parameters) =>
    {
        Debug.WriteLine(format, parameters);
    };
});
```
or you can specify your own transaction logger:
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                       .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.Name = "Example transaction";
    options.LoggerContext.Logger = new MyTransactionLogger();
});
```
* Transaction information settings
  - Transaction name: you can specify a transaction name which will be added to any logs:
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                       .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.Name = "Example transaction";
});
```
  - Transaction time provider: you can set a time provider for each DateTime dependent transaction features:
```c#
 ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                        .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.GetCurrentTimeFunction = () => this.timeProvider.Now;
});
```
  - Session id: there is a possiblity to override a session id creation operation:
```c#
ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                       .Create<WriteStepId, FileWriteData>(options =>
{
    options.TransactionInfo.SessionIdCreator = () => this.myGuidCreator.CreateGuid();
});
```
* Transaction state storage: when you specify a storage for a transaction state the transaction can be recoverable. It means that you will be able to continue a not finished transaction after an unexpected situation like an application crash or a power lost (see a point about recovering process).
 ```c#
 ITransaction<WriteStepId, FileWriteData> transaction = new TransactionFactory()
                                                        .Create<WriteStepId, FileWriteData>(options =>
 {
     options.TransactionStorageCreator = context => new MyStorage(context);
});
```
 ## Transaction run settings
* Specifying a transaction data. 
The data is injected to all transaction methods as the first parameter.
```c#
ITransactionResult<FileWriteData> result = await transaction.Run(settings => 
{
    settings.Data = new FileWriteData()
    {
        DataToWrite = new byte[] { 0x01, 0x02 },
        File = "path to the file"
    };
});
```
* Choose the run mode
There are three available modes for a transaction run which can be specified when invoking the Run method:
   - **Run**: this is a basic mode in which all steps are run in the defined order from the first one to the last one.
   - **RecoverAndUndoAndRun**: recovers the transaction (see Transaction recovering process), runs undo operations for completed steps and then starts the transaction from the first step to the last one. NOTE: If there is no session to recover the transaction is ended without run.
   - **RecoverAndContinue**: Recovers the transaction and runs not completed steps. NOTE: If there is no session to recover the transaction is ended without run.
```c#
transaction.Run(settings => 
{
    settings.Mode = RunMode.Run;
});
```
* Run callback
This is a method invoked when the transaction ends its work. When async await functionality is not available then this is the only way to get the transaction result.
```c#
transaction.Run(settings => 
{
     settings.TransactionResultCallback = result => { };
});
```
You can specify an executor (see Executors for actions) for the callback via a property TransactionResultCallbackExecutor:
```c#
transaction.Run(settings => 
{
     settings.TransactionResultCallback = result => { };
     settings.TransactionResultCallbackExecutor = new CallbackExecutor();
});
```
* Other run settings
The property contains a few other settings as flags 
   - **LogTimeExecutionForAllSteps**: time execution for each step method will be written to log.
   - **DontRecoverTransactionData**: a transaction data wouldn't be recovered (see Transaction recovering process) and will be taken from the run settings.
```c#
transaction.Run(settings => 
{
     settings.Settings = TransactionSettings.DontRecoverTransactionData
                         | TransactionSettings.LogTimeExecutionForAllSteps;
});
```
 ## Steps preparation
A step for a transaction is an object which implements the interface: BBTransaction.Step.ITransactionStep<TStepId, TData>. You can add a step to a transaction via a few methods in the transaction object: 'Add', 'InsertAtIndex', 'InsertBefore' and 'InsertAfter', i.e.:
```c#
transaction.Add(new TransactionStep<WriteStepId, FileWriteData>()
{
    Id = WriteStepId.CreateBackup,
    AsyncStepAction = CreateBackup,
    PostAction = CreateBackupPost
});
```
You can add an executor (see Executors for actions) for a step action, an undo action or a post action via properties: 'StepActionExecutor', 'UndoActionExecutor' and 'PostActionExecutor'.
You can also specify properties for a step:
```c#
transaction.Add(new TransactionStep<WriteStepId, FileWriteData>()
{
     Settings = StepSettings.UndoOnRecover
                | StepSettings.NotRunOnRecovered
});
```
* **NotRunOnRecovered**: the step should not be invoked when the transaction was recovered (see transaction recovering process)
* **UndoOnRecover**: the undo method for the step should be invoked when the step was recovered and is the first step to run
* **LogExecutionTime**: time execution for the step method will be written to log (NOTE: when you specify a setting 'LogTimeExecutionForAllSteps' for the transactoon then an execution time for all steps will be logged no matter if the step has the setting
* **SameExecutorForAllActions**: a step executor for the step action will be used for the undo and post actions if no executor was defined for the actions.
 ## Executors for actions
An executor is an object which implements an interface BBTransaction.Executor.IExecutor. It can be used to invoke a step or steps on the other thread during a transaction, i.e. imagine that you have a transaction with three steps and you want to run the second step on a specific thread. Then you can write your own executor and set it to the second step definition:
```c#
transaction.Add(new TransactionStep<string, Data>()
{
    Id = "First step",
    AsyncStepAction = Action1
});
transaction.Add(new TransactionStep<string, Data>()
{
    Id = "Second step",
    AsyncStepAction = ActionOnSpecificThread,
    StepActionExecutor = new SpecificThreadExecutor()
});
transaction.Add(new TransactionStep<string, Data>()
{
    Id = "Third step",
    AsyncStepAction = Action2
});
```
In this case the first step will be invoked on the thread on which the 'Run' method of the transaction was invoked. The second step will be invoked on the specific thread by the executor. The third step will continue run on the thread from the second step so if you want to run only one step on the specific thread you need set an additional executor which change the thread for the next step:
```c#
transaction.Add(new TransactionStep<string, Data>()
{
    Id = "First step",
    AsyncStepAction = Action1
});
transaction.Add(new TransactionStep<string, Data>()
{
    Id = "Second step",
    AsyncStepAction = ActionOnSpecificThread,
    StepActionExecutor = new SpecificThreadExecutor()
});
transaction.Add(new TransactionStep<string, Data>()
{
    Id = "Third step",
    AsyncStepAction = Action2,
    StepActionExecutor = new OtherThreadExecutor()
});
```
In this case the first step will be invoked on the thread on which the Run method of the transaction was invoked. The second step will be invoked on the other thread by the executor. The third step will be invoked on a thread provided by the OtherThreadExecutor.
 ## Transaction recovering process
A transaction object is able to continue processing steps after an applicaiton crash or a power lost. To do this you need do a few things:
1. Create a storage for a transaction state and add it to the transaction: the storage has to implement an interface BBTransaction.Transaction.Session.Storage.ITransactionStorage<TData>:
```c#
ITransaction<string, Dto> transaction = new TransactionFactory()
                                          .Create<string, Dto>(options =>
{
     options.TransactionStorageCreator = context => new MyStorage(context);
});
```
Looking on the interface there are a few methods:
* **SessionStarted**: this method is invoked when a transaction starting before the first step for run mode 'Run'
* **StepPrepared**: invoked before a step processing
* **StepReceding**: invoked before a step undo method. It indicates that the step is receding due to an error or a step move
* **RemoveSession**: invoked after the transaction end, here all states for the transaction should be removed from the storage
Let's looks on a storage example:

2. Define appropriable steps for a transaction, i.e. a transaction which appends a data to a file can be done as:

 **step 1**
 - **action**: create a backup for a destination file
 - **undo action**: switch the backup with the destination file and remove the backup
 - **post action**: remove the backup
 
 **step 2**
 - **action**: write data to the destination file
 ## Transaction run cancellation
You can cancel a transaction in a step action using a 'Cancel' method:
```c#
transaction.Add(new TransactionStep<string, Dto>()
{
     StepAction = (data, info) => info.Cancel()
});
```
When a transaction is cancelled then undo methods for all run steps are invoked and the result of the transaction is 'ResultType.Cancelled', i.e.:
when we have a transaction with steps '0', '1' and '2' and we cancel the transaction in step '1' then undo methods for steps '1' and '0' are invoked (in this order) and then the transaction ends.
 ## Moving steps back and step forward during a transaction
You can move back to a previous step from a step action using a method 'GoBack' i.e.:
```c#
transaction.Add(new TransactionStep<string, Dto>()
{
     Id = "5",
     StepAction = (data, info) => info.GoBack("1");
});
```
when we have a transaction with steps '0', '1', '2', '3', '4', '5', '6' and we move back to a step '1' from a step '5' then undo methods for steps '5', '4', '3', '2', and '1' are invoked (in this order) and then the transaction continue run from the step '1' so a step action for the step '1' is invoked and then a step action for the step '2' etc.
Move forward is simliar but in this case we only skip steps and no undo methods are invoked i.e.
```c#
transaction.Add(new TransactionStep<string, Dto>()
{
     Id = "1",
     StepAction = (data, info) => info.GoForward("5");
});
```
when we have a transaction with steps '0', '1', '2', '3', '4', '5', '6' and we move forward to a step '5' from a step '1' then after the step action for the step '1' will be invoked the step action for the step '5' so we skip step actions for steps '2', '3' and '4'.
 ## Transactions merge
There is a possibility to merge steps of two or more transactions into one transaction. To do this you need a three conversion methods:
1. A step id converter from a source transaction to the destination transaction i.e. 
when the source transaction has step ids as string and the destination transaction has step ids as int you can use a function:
```c#
int ToInt(string data)
{
    return int.Parse(data);
}
```
2. A step id converter from the destination transaction to a specific source transaction (this is the reverse conversion for point 1) i.e.
when the source transaction has step ids as string and the destination transaction has step ids as int you can use a function:
```c#
string ToString(int data)
{
    return data.ToString();
}
```
3. A converter for a transaction data i.e.
when the destination transaction has as transaction data an object:
```c#
class DestinationTransactionData
{
   ...
}
```
and the source transaction has as transaction data an object:
```c#
class SourceTransactionData
{
   ...
   public DestinationTransactionData Property { get; }
   ...
}
```
then the conversion can be done as:
```c#
DestinationTransactionData Convert(SourceTransactionData data)
{
    return data.Property;
}
```
Lets see an example:
we have two different transactions:
transaction 1:
```c#
class Transaction1Data
{
     public int IntegerProperty { get; set; }   
}

private IEnumerable<ITransactionStep<int, Transaction1Data>> Transaction1Steps
{
     get
     {
           yield return new TransactionStep<int, Transaction1Data>()
           {
               Id = 0,
               StepAction = (data, info) =>
               {
                    Console.WriteLine(string.Format(
                       "Transaction1 step '{0}', data '{1}'", 
                       info.CurrentStepId, 
                       data.IntegerProperty));
               }
            };
            yield return new TransactionStep<int, Transaction1Data>()
            {
                Id = 1,
                StepAction = (data, info) =>
                {
                     Console.WriteLine(string.Format(
                        "Transaction1 step '{0}', data '{1}'", 
                        info.CurrentStepId, 
                        data.IntegerProperty));
                }
            };
     }
}

public async Task RunTransaction1()
            {
                ITransactionResult<Transaction1Data> result = await new TransactionFactory()
                                                                .Create<int, Transaction1Data>(options => 
                {
                    options.TransactionInfo.Name = "Transaction1";
                })
                .Add(this.Transaction1Steps)
                .Run(settings => 
                {
                    settings.Data = new Transaction1Data() { IntegerProperty = 100 };
                    settings.Mode = RunMode.Run;
                });
            }
```
transaction 2:
```c#
class Transaction2Data
{        
     public string StringProperty { get; set; }

     public Transaction1Data Data { get; set; }
}

private IEnumerable<ITransactionStep<string, Transaction2Data>> Transaction2Steps
{
     get
     {
         yield return new TransactionStep<string, Transaction2Data>()
         {
              Id = "abc",
              StepAction = (data, info) =>
              {
                  Console.WriteLine(string.Format("Transaction2 step '{0}'", info.CurrentStepId));
              }
         };
         yield return new TransactionStep<string, Transaction2Data>()
         {
              Id = "def",
              StepAction = (data, info) => 
              {
                  Console.WriteLine(string.Format("Transaction2 step '{0}'", info.CurrentStepId));
              }
         };
      }
}

public async Task RunTransaction2()
{
      ITransactionResult<Transaction2Data> result = await new TransactionFactory()
                                                             .Create<string, Transaction2Data>(options =>
      {
            options.TransactionInfo.Name = "Transaction2";
      })
      .Add(this.Transaction2Steps)
      .Run(settings =>
      {
            settings.Data = new Transaction2Data()
            {
                 StringProperty = "xyz",
                 Data = new Transaction1Data() { IntegerProperty = 123 }
             };
             settings.Mode = RunMode.Run;
       });
}
```
When we run the first transaction we will see:

Transaction1 step '0', data '100'

Transaction1 step '1', data '100'

When we run the second transaction we will see:

Transaction2 step 'abc'

Transaction2 step 'def'

The merged transaction can be done as:
```c#
public async Task RunMergedTransaction()
{
       ITransactionResult<Transaction2Data> result = await new TransactionFactory()
                                                       .Create<string, Transaction2Data>(options =>
       {
            options.TransactionInfo.Name = "Merged transaction";
       })
       .Add(this.Transaction2Steps)
       .AddAdapter(
           this.Transaction1Steps, 
           transaction1StepId => transaction1StepId.ToString(), // This is the convereter from point 1
           transaction2StepId => int.Parse(transaction2StepId), // This is the converter from point 2
           transaction2Data => transaction2Data.Data) // This is the converter from point 3
        .Run(settings =>
        {
             settings.Data = new Transaction2Data()
             {
                  StringProperty = "xyz",
                  Data = new Transaction1Data() { IntegerProperty = 123 }
             };
             settings.Mode = RunMode.Run;
         });
}
```
When we run the merged transaction we will see:

Transaction2 step 'abc'

Transaction2 step 'def'

Transaction1 step '0', data '123'

Transaction1 step '1', data '123'

All steps from transaction 1 and 2 were merged into one transaction.
