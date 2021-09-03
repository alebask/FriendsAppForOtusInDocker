using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using FriendsAppNoORM.Data;
using FriendsAppNoORM.Models;
using FriendsAppNoORM.Utilities;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace FriendsAppDataGenerator{

       public interface IDataGenerator{
           public void Run();
       }

       public class DataGenerator: IDataGenerator{

        private string _mySqlConnectionString;
        private int _targetProfilesNumber = 1000000;
        private int _waitDatabaseReadyInSeconds = 60;

        private static Random random = new Random();

        public DataGenerator(IConfiguration configuration){

            _mySqlConnectionString = configuration.GetConnectionString("MySqlConnection");            
            _targetProfilesNumber = configuration.GetValue<int>("NumberOfProfilesToGenerate");
            _waitDatabaseReadyInSeconds = configuration.GetValue<int>("WaitDatabaseReadyInSeconds");
        }

        public void Run()
        {
            Console.Write("Parsing files with names... ");
            List<FirstNameRecord> firstNames = ParseNames<FirstNameRecord>("files/russian_names.csv");
            List<LastNameRecord> lastNames = ParseNames<LastNameRecord>("files/russian_surnames.csv");
            Console.WriteLine("DONE");

            Console.Write("Sorting by weight... ");
            firstNames = firstNames.OrderByDescending(x => x.PeoplesCount).ToList();
            lastNames = lastNames.OrderByDescending(x => x.PeoplesCount).ToList();
            Console.WriteLine("DONE");

            ProfileDataSet profileDataSet = new ProfileDataSet(_mySqlConnectionString);
            AccountDataSet accountDataSet = new AccountDataSet(_mySqlConnectionString);

            //one retry with some delay due db is slow to start for the first time
            long currentProfilesNumber = 0;
            try{
               currentProfilesNumber = accountDataSet.Count();
            }
            catch(MySqlException ex){
                Console.WriteLine($"Error accessing database {ex.Message}");
                Console.WriteLine($"Retry in {_waitDatabaseReadyInSeconds} seconds");
                Thread.Sleep(1000 * _waitDatabaseReadyInSeconds);

                currentProfilesNumber = accountDataSet.Count();
            }

            long count = _targetProfilesNumber - currentProfilesNumber;

            Console.Write($"Generating {count} random profiles...");

            List<Profile> profiles = new List<Profile>(_targetProfilesNumber);

            string[] femaleLastNameEndings = { "ова", "ева", "ина", "ая" };
            string[] maleLastNameEndings = { "ов", "ев", "ин", "ий" };


            for (var i = 0; i < count; i++)
            {
                FirstNameRecord fnRecord = firstNames[random.Next(0, firstNames.Count)];
                LastNameRecord lnRecord = lastNames[random.Next(0, lastNames.Count)];

                while (fnRecord.Sex == "М" && femaleLastNameEndings.Any(x => lnRecord.Surname.EndsWith(x)))
                {
                    //lnRecord = lastNamesBag.GetRandom();
                    lnRecord = lastNames[random.Next(0, lastNames.Count)];

                }
                while (fnRecord.Sex == "Ж" && maleLastNameEndings.Any(x => lnRecord.Surname.EndsWith(x)))
                {
                    //lnRecord = lastNamesBag.GetRandom();
                    lnRecord = lastNames[random.Next(0, lastNames.Count)];

                }

                Profile p = new Profile();

                p.FirstName = fnRecord.Name;
                p.LastName = lnRecord.Surname;
                p.Age = random.Next(16, 101);
                p.City = RandomRussianString(random.Next(3, 40));
                p.Gender = fnRecord.Sex;
                p.Interests = RandomRussianString(random.Next(25, 150));

                profiles.Add(p);
            }
            Console.WriteLine("DONE");


            Console.WriteLine("Inserting profiles into database...");

            string passwordHash = new PasswordHasher().Hash("Password!1");

            Stopwatch timer = new Stopwatch();

            timer.Start();

            for (var i = 0; i < profiles.Count; i++)
            {
                Profile p = profiles[i];

                Guid accountId = Guid.Empty;
                while (accountId == Guid.Empty)
                {
                    try
                    {
                        accountId = CreateAccount(accountDataSet, passwordHash);
                    }
                    catch (MySqlException ex)
                    {
                        if (ex.Message.Contains("Duplicate entry"))
                        {
                            //do nothing
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }

                profileDataSet.Create(accountId, p);

                if (i % 100 == 0)
                {
                    int speed = (int)(i / timer.Elapsed.TotalSeconds);
                    Console.WriteLine($"generated {i} profiles; speed - {speed} p/sec         ");
                }

            }

            timer.Stop();

            Console.WriteLine("\nDONE");

            Console.WriteLine($"{_targetProfilesNumber} number of profiles generated");
        }

        private Guid CreateAccount(AccountDataSet accountDataSet, string passwordHash)
        {
            Account a = new Account();
            a.Email = RandomEnglishString(random.Next(1, 10)) + "." + RandomEnglishString(random.Next(1, 20)) + "@mail.ru";
            a.PasswordHash = passwordHash;
            a = accountDataSet.Create(a);
            return a.AccountId;
        }

        private string RandomRussianString(int length)
        {

            var russianChars = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";

            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = russianChars[random.Next(russianChars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
        private string RandomEnglishString(int length)
        {

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
        private List<T> ParseNames<T>(string fileName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {

                HasHeaderRecord = true,
                Delimiter = ";"
            };

            using (TextReader reader = new StreamReader(fileName))
            {
                using (CsvReader csvReader = new CsvReader(reader, config))
                {
                    return csvReader.GetRecords<T>().ToList();
                }
            }
        }

    }
}