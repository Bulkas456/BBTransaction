using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BBTransaction.Factory;
using BBTransaction.Step;
using BBTransaction.Transaction;
using BBTransaction.Transaction.Settings;
using BBTransaction.Transaction.TransactionResult;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;

namespace BBTransactionTestsWithAsync
{
    [TestClass]
    public class AddStepsToTransactionTests
    {
        [TestMethod]
        public async Task WhenInsertStepAtIndex_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            // Act
            target.InsertAtIndex(3, new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            });
            target.InsertAtIndex(3, new TransactionStep<string, object>()
            {
                Id = "inserted 2",
                StepAction = (data, info) => runStepActions.Add("inserted 2")
            });

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[] 
            {
                "0",
                "1",
                "2",
                "inserted 2",
                "inserted 1",
                "3",
                "4",
                "5",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepsAtIndex_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            // Act
            target.InsertAtIndex(3, new TransactionStep<string, object>[] { new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            },
            new TransactionStep<string, object>()
            {
                Id = "inserted 2",
                StepAction = (data, info) => runStepActions.Add("inserted 2")
            } });

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "0",
                "1",
                "2",
                "inserted 1",
                "inserted 2",
                "3",
                "4",
                "5",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepBeforeIdWithoutComparer_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            // Act
            target.InsertBefore("0", new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            });
            target.InsertBefore("0", new TransactionStep<string, object>()
            {
                Id = "inserted 2",
                StepAction = (data, info) => runStepActions.Add("inserted 2")
            });
            target.InsertBefore("3", new TransactionStep<string, object>()
            {
                Id = "inserted 3",
                StepAction = (data, info) => runStepActions.Add("inserted 3")
            });

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "inserted 2",
                "inserted 1",
                "0",
                "1",
                "2",
                "inserted 3",
                "3",
                "4",
                "5",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepsBeforeIdWithoutComparer_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            // Act
            target.InsertBefore("1", new TransactionStep<string, object>[] {
            new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            },
            new TransactionStep<string, object>()
            {
                Id = "inserted 2",
                StepAction = (data, info) => runStepActions.Add("inserted 2")
            },
            new TransactionStep<string, object>()
            {
                Id = "inserted 3",
                StepAction = (data, info) => runStepActions.Add("inserted 3")
            }});

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "0",
                "inserted 1",
                "inserted 2",
                "inserted 3",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepBeforeIdWithComparer_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i == 3 ? i.ToString() + " " : i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            Mock<IEqualityComparer<string>> comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns((string a, string b) => a.Trim() == b.Trim());

            // Act
            target.InsertBefore("3", new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            },
            comparer.Object);

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "0",
                "1",
                "2",
                "inserted 1",
                "3 ",
                "4",
                "5",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepsBeforeIdWithComparer_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i == 3 ? i.ToString() + " " : i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            Mock<IEqualityComparer<string>> comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns((string a, string b) => a.Trim() == b.Trim());

            // Act
            target.InsertBefore("3", new TransactionStep<string, object>[] {
            new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            },
            new TransactionStep<string, object>()
            {
                Id = "inserted 2",
                StepAction = (data, info) => runStepActions.Add("inserted 2")
            }},
            comparer.Object);

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "0",
                "1",
                "2",
                "inserted 1",
                "inserted 2",
                "3 ",
                "4",
                "5",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepAfterIdWithoutComparer_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            // Act
            target.InsertAfter("0", new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            });
            target.InsertAfter("0", new TransactionStep<string, object>()
            {
                Id = "inserted 2",
                StepAction = (data, info) => runStepActions.Add("inserted 2")
            });
            target.InsertAfter("3", new TransactionStep<string, object>()
            {
                Id = "inserted 3",
                StepAction = (data, info) => runStepActions.Add("inserted 3")
            });

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "0",
                "inserted 2",
                "inserted 1",
                "1",
                "2",
                "3",
                "inserted 3",
                "4",
                "5",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepsAfterIdWithoutComparer_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            // Act
            target.InsertAfter("5", new TransactionStep<string, object>[] {
            new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            },
            new TransactionStep<string, object>()
            {
                Id = "inserted 2",
                StepAction = (data, info) => runStepActions.Add("inserted 2")
            },
            new TransactionStep<string, object>()
            {
                Id = "inserted 3",
                StepAction = (data, info) => runStepActions.Add("inserted 3")
            }});

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "inserted 1",
                "inserted 2",
                "inserted 3",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepAfterIdWithComparer_ShouldInsertProperly()
        {
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i == 4 ? i.ToString() + " " : i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            Mock<IEqualityComparer<string>> comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns((string a, string b) => a.Trim() == b.Trim());

            // Act
            target.InsertAfter("4", new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            },
            comparer.Object);

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "0",
                "1",
                "2",
                "3",
                "4 ",
                "inserted 1",
                "5",
                "6"
            });
        }

        [TestMethod]
        public async Task WhenInsertStepsAfterIdWitComparer_ShouldInsertProperly()
        {
            // Arrange
            List<string> runStepActions = new List<string>();
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });

            for (int i = 0; i < 7; ++i)
            {
                string index = i == 2 ? i.ToString() + " " : i.ToString();
                target.Add(new TransactionStep<string, object>()
                {
                    Id = index,
                    StepAction = (data, info) => runStepActions.Add(index)
                });
            }

            Mock<IEqualityComparer<string>> comparer = new Mock<IEqualityComparer<string>>();
            comparer.Setup(x => x.Equals(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns((string a, string b) => a.Trim() == b.Trim());

            // Act
            target.InsertAfter("2", new TransactionStep<string, object>[] {
            new TransactionStep<string, object>()
            {
                Id = "inserted 1",
                StepAction = (data, info) => runStepActions.Add("inserted 1")
            },
            new TransactionStep<string, object>()
            {
                Id = "inserted 2",
                StepAction = (data, info) => runStepActions.Add("inserted 2")
            },
            new TransactionStep<string, object>()
            {
                Id = "inserted 3",
                StepAction = (data, info) => runStepActions.Add("inserted 3")
            }},
            comparer.Object);

            ITransactionResult<object> result = await target.Run(settings => { });

            // Assert
            runStepActions.ShouldAllBeEquivalentTo(new string[]
            {
                "0",
                "1",
                "2 ",
                "3",
                "4",
                "5",
                "inserted 1",
                "inserted 2",
                "inserted 3",
                "6"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WhenAddStepAfterTransactionRun_ShouldThrowException()
        {
            // Arrange
            ITransaction<string, object> target = new TransactionFactory().Create<string, object>(options =>
            {
                options.TransactionInfo.Name = "test transaction";
            });
            target.Add(new TransactionStep<string, object>()
            {
                Id = "1",
                StepAction = (data, info) => { }
            });
            ITransactionResult<object> result = await target.Run(settings =>
            {
                settings.Mode = RunMode.Run;
            });

            // Act
            target.Add(new TransactionStep<string, object>()
            {
                Id = "1",
                StepAction = (data, info) => { }
            });

            // Assert
            Assert.Fail("An exception is expected.");
        }
    }
}
