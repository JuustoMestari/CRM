using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESC_CRM_
{
   public class connectionException : Exception
    {
       public connectionException() : base("Connection Failed !") { }
       public connectionException(string message) : base("Connection Failed ! Exception : "+message) { }
    }
   public class courseListException : Exception
   {
       public courseListException() : base("Get courses List Failed !") { }
       public courseListException(string message) : base("Get courses List Failed ! Exception : " + message) { }
   }
   public class courseNotFound : Exception
   {
       public courseNotFound() : base("This course doesn't exist") { }
       public courseNotFound(string message) : base("This course doesn't exist! Exception : " + message) { }
   }
   public class attributeMaxException : Exception
   {
       public attributeMaxException() : base("An error occured while getting the max value for an attribute.") { }
       public attributeMaxException(string message) : base("An error occured while getting the max value for an attribute. Exception : " + message) { }
   }
   public class mismatchException : Exception
   {
       public mismatchException() : base("An attribute conversion failed !") { }
       public mismatchException(string message) : base("An attribute conversion failed ! Exception : " + message) { }
   }
   public class attributelengthException : Exception
   {
       public attributelengthException(string message): base(message){}
       public attributelengthException(Dictionary<string,long> errorList) : base() 
       {
           string errorMSG = "";
            foreach(KeyValuePair<string,long> kvp in errorList)
            {
                errorMSG += "The attribute " + kvp.Key + " maximum size is " + kvp.Value.ToString()+"\n";
            }
            throw new attributelengthException(errorMSG);
       }
   }
}
