using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Reflection;
using UnitTestEx;
using Assert = NUnit.Framework.Assert;

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for FileStorageTest
    /// </summary>
    [TestFixture]
    public class FileStorageTest
    {
        public const string MAX_SIZE_EXCEPTION = "DIFFERENT MAX SIZE";
        public const string NULL_FILE_EXCEPTION = "NULL FILE";
        public const string NO_EXPECTED_EXCEPTION_EXCEPTION = "There is no expected exception";

        public const string SPACE_STRING = " ";
        public const string FILE_PATH_STRING = "@D:\\JDK-intellij-downloader-info.txt";
        public const string CONTENT_STRING = "Some text";
        public const string REPEATED_STRING = "AA";
        public const string WRONG_SIZE_CONTENT_STRING = "TEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtext";
        public const string TIC_TOC_TOE_STRING = "tictoctoe.game";

        public const int NEW_SIZE = 5;
        public FileStorage storage;

        // Подготовка к тесту
        [SetUp]
        public void SetUp()
        {
            storage = new FileStorage(NEW_SIZE);
        }

        /* ПРОВАЙДЕРЫ */

        static object[] NewFilesData =
            {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) },
            new object[] { new File(FILE_PATH_STRING, CONTENT_STRING) }
        };

        static object[] FilesForDeleteData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING), REPEATED_STRING },
        };

        static object[] NewExceptionFileData = {
            new object[] { new File(REPEATED_STRING, WRONG_SIZE_CONTENT_STRING) }
        };

        /* Тестирование записи файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void WriteTest(File file)
        {
            Assert.True(storage.Write(file));
        }

        /* Тестирование записи файла превышающего размер хранилища */
        [Test, TestCaseSource(nameof(NewExceptionFileData))]
        public void WriteTest_MustThrowFileIsToBigException(File file)
        {
            bool isException = false;
            try
            {
                storage.Write(file);
                Assert.False(storage.Write(file));
            }
            catch (FileIsToBigException)
            {
                isException = true;
            }
            Assert.True(isException, NO_EXPECTED_EXCEPTION_EXCEPTION);
        }

        /* Тестирование записи дублирующегося файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void WriteTest_MustThrowFileNameAlreadyExistsException (File file)
        {
            bool isException = false;
            try
            {
                storage.Write(file);
                Assert.False(storage.Write(file));
            }
            catch (FileNameAlreadyExistsException)
            {
                isException = true;
            }
            Assert.True(isException, NO_EXPECTED_EXCEPTION_EXCEPTION);
        }

        /* Тестирование проверки существования файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void IsExistsTest(File file)
        {
            String name = file.GetFilename();
            Assert.False(storage.IsExists(name));
            try
            {
                storage.Write(file);
            }
            catch (FileNameAlreadyExistsException e)
            {
                Console.WriteLine(String.Format("Exception {0} in method {1}", e.GetBaseException(), MethodBase.GetCurrentMethod().Name));
            }
            Assert.True(storage.IsExists(name));
        }

        /* Тестирование удаления файла */
        [Test, TestCaseSource(nameof(FilesForDeleteData))]
        public void DeleteTest(File file, String fileName)
        {
            storage.Write(file);
            Assert.True(storage.Delete(fileName));
        }
        // Тестирование удаления несущесвующего файла
        [Test, TestCaseSource(nameof(FilesForDeleteData))]
        public void DeleteTest_MustReturnFalse(File file, String fileName)
        {
            Assert.False(storage.Delete(fileName));
        }

        //Тестирование удаления всех файлов
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void DeleteAllFilesTest(File file)
        {
            storage.Write(file);
            Assert.True(storage.DeleteAllFiles());
            Assert.Zero(storage.GetFiles().Count);
        }

        /* Тестирование получения файлов */
        [Test]
        public void GetFilesTest()
        {
            foreach (File file in storage.GetFiles())
            {
                Assert.NotNull(file);
            }
        }

        // Почти эталонный
        /* Тестирование получения файла */
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void GetFileTest(File expectedFile)
        {
            storage.Write(expectedFile);

            File actualfile = storage.GetFile(expectedFile.GetFilename());
            bool difference = actualfile.GetFilename().Equals(expectedFile.GetFilename()) && actualfile.GetSize().Equals(expectedFile.GetSize());

            Assert.IsTrue(difference, string.Format("There is some differences in {0} or {1}", expectedFile.GetFilename(), expectedFile.GetSize()));
        }
    }
}
