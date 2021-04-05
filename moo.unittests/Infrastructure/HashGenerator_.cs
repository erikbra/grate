using System.Threading.Tasks;
using moo.Infrastructure;
using NUnit.Framework;

namespace moo.unittests.Infrastructure
{
    [TestFixture]
    public class HashGenerator_
    {
        [Test]
        public async Task Generates_the_correct_hash()
        {
            string text_to_hash = "I want to see what the freak is going on here";
            string expected_hash = "TMGPZJmBhSO5uYbf/TBqNA==";

            var hashGen = new HashGenerator();
            Assert.AreEqual(expected_hash, hashGen.Hash(text_to_hash));
        }
    }
}