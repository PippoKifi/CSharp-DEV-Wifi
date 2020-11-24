﻿using AbstrakteKlassen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacesGL
{
    class Program
    {
        static void Main(string[] args)
        {
            //IDataRepository repository = new MemoryRepository(200);
            IDataRepository repository = new TextFileRepository("myData.txt", 200);

            //daten speichern
            PersistMyData(repository, "Hallo Welt! Dies ist eine Testdatensatz. ");

            //daten lesen
            string myData = ReadMyData(repository);

            //ausgabe
            Console.WriteLine(myData);
        }

        static string ReadMyData(IDataRepository repository)
        {
            if (repository != null)
            {
                Console.WriteLine(repository);
                return repository.Read();
            }

            return string.Empty;
        }

        static void PersistMyData(IDataRepository repository, string dataToPersist)
        {
            if (repository != null)
            {
                Console.WriteLine(repository);
                repository.Write(dataToPersist);
            }
        }
    }
}
