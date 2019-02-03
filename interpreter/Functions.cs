using System;
using System.Globalization;
using System.Linq;

namespace Interpreter1
{
    internal class Add : InterpreterFunc
    {
        public Add(ExprContext context) : base(context)
        {}
        
        public override Value Execute()
        {
            float added = 0;
            Types returnType = Types.Int;
            Context.Arguments.ForEach(lazy =>
            {
                var a = lazy.Execute();
                if (a.Type == Types.FloatingPoint)
                {
                    added += float.Parse(a.Val, CultureInfo.InvariantCulture);
                    returnType = Types.FloatingPoint;
                }
                else
                {
                    added += int.Parse(a.Val);
                }
            });

            return new Value(added.ToString(CultureInfo.InvariantCulture), returnType);
        }
    }

    internal class Sub : InterpreterFunc
    {
        public Sub(ExprContext context) : base(context)
        {
        }
        
        public override Value Execute()
        {
            float subbed = 0;
            Types returnType = Types.Int;
            for (var i = 0; i < Context.Arguments.Count; i++)
            {
                var a = Context.Arguments[i].Execute();
                if (a.Type == Types.FloatingPoint)
                {
                    var val = float.Parse(a.Val.ToString(), CultureInfo.InvariantCulture);
                    if (i == 0)
                        subbed = val;
                    else
                        subbed -= val;
                    returnType = Types.FloatingPoint;
                }
                else
                {
                    var val = int.Parse(a.Val.ToString());
                    if (i == 0)
                        subbed = val;
                    else
                        subbed -= val;
                }
            }

            return new Value(subbed.ToString(CultureInfo.InvariantCulture), returnType);
        }
    }

    internal class Div : InterpreterFunc
    {
        public Div(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            float divd = 0;
            Types returnType = Types.FloatingPoint;
            for (var i = 0; i < Context.Arguments.Count; i++)
            {
                var a = Context.Arguments[i].Execute();
                if (a.Type == Types.FloatingPoint)
                {
                    var val = float.Parse(a.Val.ToString(), CultureInfo.InvariantCulture);
                    if (i == 0)
                        divd = val;
                    else
                        divd /= val;
                }
                else
                {
                    var val = int.Parse(a.Val.ToString());
                    if (i == 0)
                        divd = val;
                    else
                        divd /= val;
                }
            }

            return new Value(divd.ToString(CultureInfo.InvariantCulture), returnType);
        }
    }

    internal class Mult : InterpreterFunc
    {
        public Mult(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            float mult = 0;
            Types returnType = Types.FloatingPoint;
            for (var i = 0; i < Context.Arguments.Count; i++)
            {
                var a = Context.Arguments[i].Execute();
                if (a.Type == Types.FloatingPoint)
                {
                    var val = float.Parse(a.Val.ToString(), CultureInfo.InvariantCulture);
                    if (i == 0)
                        mult = val;
                    else
                        mult *= val;
                }
                else
                {
                    var val = int.Parse(a.Val.ToString());
                    if (i == 0)
                        mult = val;
                    else
                        mult *= val;
                }
            }

            return new Value(mult.ToString(CultureInfo.InvariantCulture), returnType);
        }
    }

    internal class StatementsGroup : InterpreterFunc
    {
        public StatementsGroup(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count == 0)
                throw new WrongArgumentCount(Context.FunctionName, 1);

            Context.Arguments.GetRange(0, Context.Arguments.Count - 1).ForEach((a) => a.Execute());
            return Context.Arguments.Last().Execute();
        }
    }

    internal class Print : InterpreterFunc
    {
        public Print(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count < 1)
                throw new WrongArgumentCount("ExprContext.FunctionName", 1);

            var fullStr = "";
            Value last = null;
            foreach (var lazy in Context.Arguments)
            {
                var argument = lazy.Execute();
                if (argument.Type != Types.String && argument.Type != Types.Int && argument.Type != Types.FloatingPoint)
                {
                    throw new WrongType(Context.FunctionName, "", Types.String, Types.Int, Types.FloatingPoint);
                }
                fullStr += argument.Val;
                last = argument;
            }

            Console.WriteLine(fullStr.Replace("\"", string.Empty));
            return last;
        }
    }

    internal class Read : InterpreterFunc
    {
        public Read(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count != 0)
                throw new WrongArgumentCount(Context.FunctionName, 0, 0);

            var read = Console.ReadLine();
            // HACK pseudo flush for repl
            if (read == string.Empty)
            {
                read = Console.ReadLine();
            }
            return new Value(read, Types.String);
        }
    }

    internal class TypeOf : InterpreterFunc
    {
        public TypeOf(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count != 1)
                throw new WrongArgumentCount(Context.FunctionName, 1, 1);

            return new Value(Context.Arguments.First().Execute().Type.ToString(), Types.String);
        }
    }

    internal class Eq : InterpreterFunc
    {
        public Eq(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count != 2)
                throw new WrongArgumentCount(Context.FunctionName, 2, 2);

            if (Context.Arguments.First().Execute().Val.Equals(Context.Arguments.Last().Execute().Val))
            {
                return new Value("t", Types.Name);
            }
            return new Value("()", Types.EmptyList);
        }
    }

    internal class Not : InterpreterFunc
    {
        public Not(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count != 1)
                throw new WrongArgumentCount(Context.FunctionName, 1, 1);

            if (Context.Arguments.First().Execute().Type == Types.EmptyList)
            {
                return new Value("t", Types.Name);
            }
            return new Value("()", Types.EmptyList);
        }
    }

    internal class When : InterpreterFunc
    {
        public When(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count != 2)
                throw new WrongArgumentCount(Context.FunctionName, 2, 2);

            if (Context.Arguments.First().Execute().Type != Types.EmptyList)
            {
                return Context.Arguments.Last().Execute();
            }
            else
            {
                return new Value("()", Types.EmptyList);
            }
        }
    }
    
    internal class If : InterpreterFunc
    {
        public If(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count != 3)
                throw new WrongArgumentCount("if", 3, 3);

            if (Context.Arguments.First().Execute().Type != Types.EmptyList)
            {
                return Context.Arguments[1].Execute();
            }
            return Context.Arguments.Last().Execute();
        }
    }

    internal class Def : InterpreterFunc
    {
        public Def(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count != 3)
                throw new WrongArgumentCount(Context.FunctionName, 3, 3);

            if (Context.Arguments.First().Execute().Type != Types.Name)
                throw new WrongType(Context.FunctionName, "first argument should be a Name", Types.Name);

            Context
                .AddValueToLocalContext(
                    Context.Arguments.First().Execute().Val, 
                    Context.Arguments[1].Execute());
            
            return Context.Arguments.Last().Execute();
        }
    }

    internal class Retrieve : InterpreterFunc
    {
        public Retrieve(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count != 1)
                throw new WrongArgumentCount(Context.FunctionName, 1, 1);

            var value = Context.Arguments.First().Execute();
            if (value.Type != Types.Name)
                throw new WrongType(Context.FunctionName, " argument should be a Name", Types.Name);

            return Context.RetrieveValueFromContext(value.Val);
        }
    }

    //Function with no argument
    internal class DefineFunction : InterpreterFunc
    {
        public DefineFunction(ExprContext context) : base(context)
        {
        }

        public override Value Execute()
        {
            if (Context.Arguments.Count < 2)
                throw new WrongArgumentCount(Context.FunctionName, 2);

            var funcName = Context.Arguments.First().Execute();
            if (funcName.Type != Types.Name)
                throw new WrongType(Context.FunctionName, "first argument should be a Name", Types.Name);
            
            //let's remove name + body from the context
            var codeBody = Context.Arguments.Last();
            Context.Arguments.RemoveAt(0);
            Context.Arguments.Remove(codeBody); 
            
            Context.AddFunctionToLocalContext(
                funcName.Val, 
                (context) => new CustomFunc(context, funcName.Val, codeBody, Context));
            
            return new Value("t", Types.Name);
        }
    }

    internal class CustomFunc : InterpreterFunc
    {
        private readonly LazyValue _codeBody;
        private readonly string _functionName;
        private readonly ExprContext _funcDeclContext;

        public CustomFunc(ExprContext context, string funcName, LazyValue codeBody, ExprContext funcDeclContext) : base(context)
        {
            _codeBody = codeBody;
            _funcDeclContext = funcDeclContext;
            _functionName = funcName;  
        }
        
        public override Value Execute()
        {
            var funcArgs = _funcDeclContext.Arguments;
            if (funcArgs.Count != Context.Arguments.Count)
                throw new WrongArgumentCount(_functionName, Context.Arguments.Count);
                
            for (var i = 0; i < Context.Arguments.Count; i++)
            {
                var name = _funcDeclContext.Arguments[i].Execute().Val;
                var val = Context.Arguments[i].Execute();
                //_funcDeclContext.Clean(); // TODO
                _funcDeclContext.AddValueToLocalContext(name, val);
            }

            return _codeBody.Execute();
        }
    }
}