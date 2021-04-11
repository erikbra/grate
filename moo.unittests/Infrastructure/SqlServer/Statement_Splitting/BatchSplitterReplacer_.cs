using Microsoft.Extensions.Logging.Abstractions;
using moo.Infrastructure;
using moo.Migration;
using moo.unittests.TestInfrastructure;
using NUnit.Framework;
// ReSharper disable InconsistentNaming

namespace moo.unittests.Infrastructure.SqlServer.Statement_Splitting
{
    [TestFixture]
    public class BatchSplitterReplacer_
    {
        private const string batch_terminator_replacement_string = StatementSplitter.BatchTerminatorReplacementString;

        private const string symbols_to_check = "`~!@#$%^&*()-_+=,.;:'\"[]\\/?<>";
        private const string words_to_check = "abcdefghijklmnopqrstuvwzyz0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static readonly IDatabase Database = new SqlServerDatabase(NullLogger<SqlServerDatabase>.Instance);
        private static BatchSplitterReplacer Replacer => new(Database.StatementSeparatorRegex, StatementSplitter.BatchTerminatorReplacementString);
        
        public class should_replace_on
        {
            [Test]
            public void full_statement_without_issue()
            {
                string sql_to_match = SplitterContext.FullSplitter.tsql_statement;
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(SplitterContext.FullSplitter.tsql_statement_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_space()
            {
                const string sql_to_match = @" GO ";
                string expected_scrubbed = @" " + batch_terminator_replacement_string + @" ";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_tab()
            {
                string sql_to_match = @" GO" + string.Format("\t");
                string expected_scrubbed = @" " + batch_terminator_replacement_string + string.Format("\t");
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_by_itself()
            {
                const string sql_to_match = @"GO";
                string expected_scrubbed = batch_terminator_replacement_string;
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_starting_file()
            {
                const string sql_to_match = @"GO
whatever";
                string expected_scrubbed = batch_terminator_replacement_string + @"
whatever";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_new_line()
            {
                const string sql_to_match = @" GO
";
                string expected_scrubbed = @" " + batch_terminator_replacement_string + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_on_new_line_after_double_dash_comments()
            {
                const string sql_to_match =
                    @"--
GO
";
                string expected_scrubbed =
                    @"--
" + batch_terminator_replacement_string + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_on_new_line_after_double_dash_comments_and_words()
            {
                string sql_to_match = @"-- " + words_to_check + @"
GO
";
                string expected_scrubbed = @"-- " + words_to_check + @"
" + batch_terminator_replacement_string + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_new_line_after_double_dash_comments_and_symbols()
            {
                string sql_to_match = @"-- " + symbols_to_check + @"
GO
";
                string expected_scrubbed = @"-- " + symbols_to_check + @"
" + batch_terminator_replacement_string + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_on_its_own_line()
            {
                const string sql_to_match = @" 
GO
";
                string expected_scrubbed = @" 
" + batch_terminator_replacement_string + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_no_line_terminator()
            {
                const string sql_to_match = @" GO ";
                string expected_scrubbed = @" " + batch_terminator_replacement_string + @" ";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_words_before()
            {
                string sql_to_match = words_to_check + @" GO
";
                string expected_scrubbed = words_to_check + @" " + batch_terminator_replacement_string + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_symbols_and_words_before()
            {
                string sql_to_match = symbols_to_check + words_to_check + @" GO
";
                string expected_scrubbed = symbols_to_check + words_to_check + @" " +
                                           batch_terminator_replacement_string + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_words_and_symbols_before()
            {
                string sql_to_match = words_to_check + symbols_to_check + @" GO
";
                string expected_scrubbed = words_to_check + symbols_to_check + @" " +
                                           batch_terminator_replacement_string + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_words_after_on_the_same_line()
            {
                string sql_to_match = @" GO " + words_to_check;
                string expected_scrubbed = @" " + batch_terminator_replacement_string + @" " + words_to_check;
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_words_after_on_the_same_line_including_symbols()
            {
                string sql_to_match = @" GO " + words_to_check + symbols_to_check;
                string expected_scrubbed = @" " + batch_terminator_replacement_string + @" " + words_to_check +
                                           symbols_to_check;
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_words_before_and_after_on_the_same_line()
            {
                string sql_to_match = words_to_check + @" GO " + words_to_check;
                string expected_scrubbed = words_to_check + @" " + batch_terminator_replacement_string + @" " +
                                           words_to_check;
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_words_before_and_after_on_the_same_line_including_symbols()
            {
                string sql_to_match = words_to_check + symbols_to_check.Replace("'", "").Replace("\"", "") +
                                      " GO BOB" + symbols_to_check;
                string expected_scrubbed = words_to_check + symbols_to_check.Replace("'", "").Replace("\"", "") +
                                           " " + batch_terminator_replacement_string + " BOB" + symbols_to_check;
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_after_double_dash_comment_with_single_quote_and_single_quote_after_go()
            {
                string sql_to_match = words_to_check + @" -- '
GO
select ''
go";
                string expected_scrubbed = words_to_check + @" -- '
" + batch_terminator_replacement_string + @"
select ''
" + batch_terminator_replacement_string;
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_comment_after()
            {
                string sql_to_match = " GO -- comment";
                string expected_scrubbed = " " + batch_terminator_replacement_string + " -- comment";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }
            
            [Test]
            public void go_with_semicolon_directly_after()
            {
                string sql_to_match = "jalla GO;";
                string expected_scrubbed = "jalla " + batch_terminator_replacement_string + ";";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }
         
        }

        public class should_not_replace_on
        {
            [Test]
            public void g()
            {
                const string sql_to_match = @" G
";
                const string expected_scrubbed = @" G
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void o()
            {
                const string sql_to_match = @" O
";
                const string expected_scrubbed = @" O
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_when_go_is_the_last_part_of_the_last_word_on_a_line()
            {
                string sql_to_match = words_to_check + @"GO
";
                string expected_scrubbed = words_to_check + @"GO
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_starting_line()
            {
                string sql_to_match = @"--GO
";
                string expected_scrubbed = @"--GO
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_and_space_starting_line()
            {
                string sql_to_match = @"-- GO
";
                string expected_scrubbed = @"-- GO
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_and_space_starting_line_and_words_after_go()
            {
                string sql_to_match = @"-- GO " + words_to_check + @"
";
                string expected_scrubbed = @"-- GO " + words_to_check + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_and_space_starting_line_and_symbols_after_go()
            {
                string sql_to_match = @"-- GO " + symbols_to_check + @"
";
                string expected_scrubbed = @"-- GO " + symbols_to_check + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_and_tab_starting_line()
            {
                string sql_to_match = @"--" + string.Format("\t") + @"GO
";
                string expected_scrubbed = @"--" + string.Format("\t") + @"GO
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_and_tab_starting_line_and_words_after_go()
            {
                string sql_to_match = @"--" + string.Format("\t") + @"GO " + words_to_check + @"
";
                string expected_scrubbed = @"--" + string.Format("\t") + @"GO " + words_to_check + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_and_tab_starting_line_and_symbols_after_go()
            {
                string sql_to_match = @"--" + string.Format("\t") + @"GO " + symbols_to_check + @"
";
                string expected_scrubbed = @"--" + string.Format("\t") + @"GO " + symbols_to_check + @"
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_starting_line_with_words_before_go()
            {
                string sql_to_match = @"-- " + words_to_check + @" GO
";
                string expected_scrubbed = @"-- " + words_to_check + @" GO
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_when_between_tick_marks()
            {
                const string sql_to_match = @"' GO
            '";
                const string expected_scrubbed = @"' GO
            '";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void
                go_when_between_tick_marks_with_symbols_and_words_before_ending_on_same_line()
            {
                string sql_to_match = @"' " + symbols_to_check.Replace("'", string.Empty) + words_to_check + @" GO'";
                string expected_scrubbed =
                    @"' " + symbols_to_check.Replace("'", string.Empty) + words_to_check + @" GO'";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_when_between_tick_marks_with_symbols_and_words_before()
            {
                string sql_to_match = @"' " + symbols_to_check.Replace("'", string.Empty) + words_to_check + @" GO
            '";
                string expected_scrubbed = @"' " + symbols_to_check.Replace("'", string.Empty) + words_to_check + @" GO
            '";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_when_between_tick_marks_with_symbols_and_words_after()
            {
                string sql_to_match = @"' GO
            " + symbols_to_check.Replace("'", string.Empty) + words_to_check + @"'";
                string expected_scrubbed = @"' GO
            " + symbols_to_check.Replace("'", string.Empty) + words_to_check + @"'";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_with_double_dash_comment_starting_line_with_symbols_before_go()
            {
                string sql_to_match = @"--" + symbols_to_check + @" GO
";
                string expected_scrubbed = @"--" + symbols_to_check + @" GO
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void
                go_with_double_dash_comment_starting_line_with_words_and_symbols_before_go()
            {
                string sql_to_match = @"--" + symbols_to_check + words_to_check + @" GO
";
                string expected_scrubbed = @"--" + symbols_to_check + words_to_check + @" GO
";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_inside_of_comments()
            {
                string sql_to_match = @"/* GO */";
                string expected_scrubbed = @"/* GO */";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_inside_of_comments_with_a_line_break()
            {
                string sql_to_match = @"/* GO 
*/";
                string expected_scrubbed = @"/* GO 
*/";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_inside_of_comments_with_words_before()
            {
                string sql_to_match =
                    @"/* 
" + words_to_check + @" GO

*/";
                string expected_scrubbed =
                    @"/* 
" + words_to_check + @" GO

*/";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_inside_of_comments_with_words_before_on_a_different_line()
            {
                string sql_to_match =
                    @"/* 
" + words_to_check + @" 
GO

*/";
                string expected_scrubbed =
                    @"/* 
" + words_to_check + @" 
GO

*/";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_inside_of_comments_with_words_before_and_after_on_different_lines()
            {
                string sql_to_match =
                    @"/* 
" + words_to_check + @" 
GO

" + words_to_check + @"
*/";
                string expected_scrubbed =
                    @"/* 
" + words_to_check + @" 
GO

" + words_to_check + @"
*/";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }

            [Test]
            public void go_inside_of_comments_with_symbols_after_on_different_lines()
            {
                string sql_to_match =
                    @"/* 
GO

" + symbols_to_check + @" 
*/";
                string expected_scrubbed =
                    @"/* 
GO

" + symbols_to_check + @" 
*/";
                TestContext.WriteLine(sql_to_match);
                string sql_statement_scrubbed = Replacer.Replace(sql_to_match);
                Assert.AreEqual(expected_scrubbed, sql_statement_scrubbed);
            }
        }

    }
}