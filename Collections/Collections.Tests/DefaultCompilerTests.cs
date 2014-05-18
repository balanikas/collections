using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collections.Compiler;
using NUnit.Framework;

namespace Collections.Tests
{
    [TestFixture]
    public class DefaultCompilerTests
    {
        private string _validCode = @"class X {}";
        private string _invalidCode = @"invalid code";

        [Test]
        public void DefaultCompiler_CompileValidCode()
        {
            var compiler = new DefaultCompiler();
            var compiledAssembly = compiler.Compile(_validCode);
            Assert.IsNotNull(compiledAssembly);
            
        }

        [Test]
        public void DefaultCompiler_CompileInvalidCode()
        {
            var compiler = new DefaultCompiler();
            Assert.That(() => compiler.Compile(_invalidCode), Throws.Exception);
            
        }


        [Test]
        public void DefaultCompiler_TryCompileValidCode()
        {
            var compiler = new DefaultCompiler();
            List<string> errors;
            var result = compiler.TryCompile(_validCode, out errors);

            Assert.IsNotNull(result);
            CollectionAssert.IsEmpty(errors);
        }

        [Test]
        public void DefaultCompiler_TryCompileInvalidCode()
        {
            var compiler = new DefaultCompiler();
            List<string> errors;
            var result = compiler.TryCompile(_invalidCode, out errors);

            Assert.IsNull(result);
            CollectionAssert.IsNotEmpty(errors);
        }
    }
}
