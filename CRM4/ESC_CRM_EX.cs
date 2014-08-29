using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ESC_CRM_EX
/// </summary>
namespace ESC_CRM_EX
{
    public class entitynotfoundException : System.Exception
    {
        public entitynotfoundException() : base("!entitynotfoundException!\nNo entity found.") { }
        public entitynotfoundException(string message) : base(message) { }
        public entitynotfoundException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected entitynotfoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class notConnectedException : System.Exception
    {

        public notConnectedException() : base("!notConnectedException!\nYou have to use the connect() method first !") { }
        public notConnectedException(string message) : base(message) { }
        public notConnectedException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected notConnectedException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class createPropertyException : System.Exception
    {
        public createPropertyException() : base() { }
        public createPropertyException(string message, string propertyName)
            : base("!createPropertyException!\nFailed to create the " + propertyName + "\nException generated :" + message)
        {
        }
        public createPropertyException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected createPropertyException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class addPropertyException : System.Exception
    {
        public addPropertyException() : base() { }
        public addPropertyException(string message, string propertyName)
            : base("!addPropertyException!\nFailed to add the property \"" + propertyName + "\"\nException generated :" + message)
        {
        }
        public addPropertyException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected addPropertyException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class typenotfound : System.Exception
    {
        public typenotfound() : base() { }
        public typenotfound(string message, string typeName)
            : base("!typenotfoundException!\nFailed to find the type \"" + typeName + "\"\nException generated :" + message)
        {
        }
        public typenotfound(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected typenotfound(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class addEntityException : System.Exception
    {
        public addEntityException() : base() { }
        public addEntityException(string message, string entityName)
            : base("!addEntityException!\nFailed to add the entity \"" + entityName + "\"\nException generated :" + message)
        {
        }
        public addEntityException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected addEntityException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class getEntityException : System.Exception
    {
        public getEntityException() : base() { }
        public getEntityException(string message, string entityName, Guid entityGUID)
            : base("!getEntityException!\nFailed to retrieve the entity \"" + entityName + "\" with the GUID :\"" + entityGUID.ToString() + "\"\nException generated :" + message)
        {
        }
        public getEntityException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected getEntityException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class deleteEntityException : System.Exception
    {
        public deleteEntityException() : base() { }
        public deleteEntityException(string message, string entityName, Guid entityGUID)
            : base("!deleteEntityException!\nFailed to delete the entity \"" + entityName + "\" with the GUID :\"" + entityGUID.ToString() + "\"\nException generated :" + message)
        {
        }
        public deleteEntityException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected deleteEntityException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class updateEntityException : System.Exception
    {
        public updateEntityException() : base() { }
        public updateEntityException(string message, string entityName, Guid entityGUID, string primaryKey)
            : base("!updateEntityException!\nFailed to update the entity \"" + entityName + "\" with the GUID :\"" + entityGUID.ToString() + "\" for \"" + primaryKey + "\"\nException generated :" + message)
        {
        }
        public updateEntityException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected updateEntityException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public class createStringConditionException : System.Exception
    {
        public createStringConditionException() : base() { }
        public createStringConditionException(string message)
            : base("!createStringConditionException!\nFailed to create the string condition expression !\nExpression generated :" + message)
        {
        }
        public createStringConditionException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected createStringConditionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}