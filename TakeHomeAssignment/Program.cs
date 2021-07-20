using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TakeHomeAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Console.WriteLine("Pleaser enter an XML filename that you need to search. ");
            //Remove the extra spaces from the filename if the user accidentally adds additional spaces
            string fileName = (Console.ReadLine()).Replace(" ", "");

            //Get the current directory of the project which is located under bin folder
            var currentDirectory = Directory.GetCurrentDirectory();
			
            //Trim all the unnecessary parts of the current directory to get to the root directory of the project
            //Assume all the xml files are stored somewhere in the main project's folder which is treated as a root directory
            string rootDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName;

            //Call SearchFile method to look for the filename in the project's root directory and its subdirectories
            List<string> filePath = SearchFile(rootDirectory, fileName);


            //Handles duplicate files, found file, and non-existing file
            if (filePath.Count > 1)
            {
                Console.WriteLine("Duplicate file is found!");
            }
            else if (filePath.Count == 1)
            {
                validateEmailAddress(filePath[0]);
            }
            else
            {
                Console.WriteLine("File doesn't exist");
            }

        }

        //Recursively search for the file in the root directory and its subdirectories
        //Returns the list of file's path(s)
        static List<string> SearchFile(string rootDirectory, string fileName)
        {
            List<string> fileList = new List<string>();
            //Search in the root directory first
            foreach (var file in Directory.EnumerateFiles(rootDirectory).Where(m => m.Contains(fileName)))
            {
                fileList.Add(file);
            }

            //Get all the subdirectories of the provided directory. Then, search for the filename
            foreach (var subDir in Directory.EnumerateDirectories(rootDirectory))
            {

                fileList.AddRange(SearchFile(subDir, fileName));
            }

            return fileList;
        }

        //Validate email addresses and put them in seperate lists
        //Then, display the lists to the user
        static void validateEmailAddress(string filePath)
        {
            List<string> validEmailAddressList = new List<string>();
            List<string> invalidEmailAddressList = new List<string>();
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                     @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                     @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");


            XDocument doc = XDocument.Load(filePath);
            foreach (XElement element in doc.Root.Elements())
            {
                //Console.WriteLine(element.Element("emailAddress").Value);
                if (regex.IsMatch(element.Element("emailAddress").Value))
                {
                    validEmailAddressList.Add(element.Element("emailAddress").Value);
                }
                else
                {
                    invalidEmailAddressList.Add(element.Element("emailAddress").Value);
                }

            }

            Console.WriteLine("Valid email addresses are: ");
            foreach (string s in validEmailAddressList)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine();
            Console.WriteLine("Invalid email addresses are: ");
            foreach (string s in invalidEmailAddressList)
            {
                Console.WriteLine(s);
            }
        }
    }
}
