using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;


namespace Fare.IntegrationTests
{

    /// <summary>Regular expression patterns for tests</summary>
    public class RegExPatternTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "[ab]{4,6}" };
            yield return new object[] { "[ab]{4,6}c" };
            yield return new object[] { "(a|b)*ab" };
            yield return new object[] { "[A-Za-z0-9]" };
            yield return new object[] { "[A-Za-z0-9_]" };
            yield return new object[] { "[A-Za-z]" };
            yield return new object[] { "[ \t]" };
            yield return new object[] { @"[(?<=\W)(?=\w)|(?<=\w)(?=\W)]" };
            yield return new object[] { "[\x00-\x1F\x7F]" };
            yield return new object[] { "[0-9]" };
            yield return new object[] { "[^0-9]" };
            yield return new object[] { "[\x21-\x7E]" };
            yield return new object[] { "[a-z]" };
            yield return new object[] { "[\x20-\x7E]" };
            yield return new object[] { "[ \t\r\n\v\f]" };
            yield return new object[] { "[^ \t\r\n\v\f]" };
            yield return new object[] { "[A-Z]" };
            yield return new object[] { "[A-Fa-f0-9]" };
            yield return new object[] { "in[du]" };
            yield return new object[] { "x[0-9A-Z]" };
            yield return new object[] { "[^A-M]in" };
            yield return new object[] { ".gr" };
            yield return new object[] { @"\(.*l" };
            yield return new object[] { "W*in" };
            yield return new object[] { "[xX][0-9a-z]" };
            yield return new object[] { @"\(\(\(ab\)*c\)*d\)\(ef\)*\(gh\)\{2\}\(ij\)*\(kl\)*\(mn\)*\(op\)*\(qr\)*" };
            yield return new object[] { @"((mailto\:|(news|(ht|f)tp(s?))\://){1}\S+)" };
            yield return new object[] { @"^http\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$" };
            yield return new object[] { @"^([1-zA-Z0-1@.\s]{1,255})$" };
            yield return new object[] { "[A-Z][0-9A-Z]{10}" };
            yield return new object[] { "[A-Z][A-Za-z0-9]{10}" };
            yield return new object[] { "[A-Za-z0-9]{11}" };
            yield return new object[] { "[A-Za-z]{11}" };
            yield return new object[] { @"^[a-zA-Z''-'\s]{1,40}$" };
            yield return new object[] { @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$" };
            yield return new object[] { @"a[a-z]" };
            yield return new object[] { @"[1-9][0-9]" };
            yield return new object[] { @"\d{8}" };
            yield return new object[] { @"\d{5}(-\d{4})?" };
            yield return new object[] { @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}" };
            yield return new object[] { @"\D{8}" };
            yield return new object[] { @"\D{5}(-\D{4})?" };
            yield return new object[] { @"\D{1,3}\.\D{1,3}\.\D{1,3}\.\D{1,3}" };
            yield return new object[] { @"^(?:[a-z0-9])+$" };
            yield return new object[] { @"^(?i:[a-z0-9])+$" };
            yield return new object[] { @"^(?s:[a-z0-9])+$" };
            yield return new object[] { @"^(?m:[a-z0-9])+$" };
            yield return new object[] { @"^(?n:[a-z0-9])+$" };
            yield return new object[] { @"^(?x:[a-z0-9])+$" };
            yield return new object[] { "\\S+.*" };
            yield return new object[] { @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$" };
            yield return new object[] { @"^\s1\s+2\s3\s?4\s*$" };
            yield return new object[] { @"(\s123)+" };
            yield return new object[] { @"\Sabc\S{3}111" };
            yield return new object[] { @"^\S\S  (\S)+$" };
            yield return new object[] { @"\\abc\\d" };
            yield return new object[] { @"\w+1\w{4}" };
            yield return new object[] { @"\W+1\w?2\W{4}" };
            yield return new object[] { @"^[^$]$" };
        }
    }

    
    /// <summary>Regular expression patterns with expected expanded pattern</summary>
    public class ExpressionWithExpansionTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            /* TODO: the expanded versions seem incorrect or too heavy on parens
                e.g., should 'a' expand to '\a'?  '9' to '\9'?
            */
            yield return new object[] { @"a", @"\a" };
            yield return new object[] { @"\d", @"[\0-\9]" };
            yield return new object[] { @"\a", @"\a" };
            yield return new  object[] { @"[ab]{4,6}", @"((\a|\b)){4,6}" };
            yield return new  object[] { @"[ab]{4,6}c", @"((\a|\b)){4,6}\c" };
            yield return new object[] { @"\w", @"(((\_|[\A-\Z])|[\a-\z])|[\0-\9])" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


}