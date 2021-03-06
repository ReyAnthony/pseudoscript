﻿using System;
using System.Linq;

namespace GMinusMinus.interpreter
{
    public class UndefinedFunction : Exception
    {
        public UndefinedFunction(string name) : base(string.Format("The function '{0}' is undefined.", name))
        {
            
        }
    }
    
    public class UndefinedVariable : Exception
    {
        public UndefinedVariable(string name) : base(string.Format("The variable '{0}' is undefined.", name))
        {
            
        }
    }
    
    public class WrongArgumentCount : Exception
    {
        public WrongArgumentCount(string funcName, int minArgs, int maxArgs = Int32.MaxValue) 
            : base(string.Format("'{0}' must have at least {1} argument(s) and at most {2} argument(s)", funcName,
                minArgs, maxArgs))
        {
        }
    }
    
    public class WrongType : Exception
    {
        public WrongType(string funcName, string msg = "", params Types[] excpectedTypes) 
            : base(string.Format("{0} needs ", funcName) +
                   string.Format("{0} types ",
                   excpectedTypes.Aggregate("", (str, next) => string.Format("{0}{1}, ", str, next)).TrimEnd(',', ' ')) +
                   string.Format("{0}", msg))
        {
        }
    }
    
    public class TopLevelFunctionDeclaration : Exception
    {
        public TopLevelFunctionDeclaration() : base("A function cannot be defined at the toplevel.")
        {       
        }
    }
}