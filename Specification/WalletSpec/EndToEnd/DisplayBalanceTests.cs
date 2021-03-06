﻿namespace Specification.WalletSpec.EndToEnd
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    class DisplayBalanceTests
    {
        private EndToEndTester _endToEnd;

        [SetUp]
        public void Setup()
        {
            _endToEnd = new EndToEndTester();
        }

        [Test]
        public void ShouldDisplayBalanceOfAllSources()
        {
            //given
            _endToEnd.Execute("/wallet add source 2");
            _endToEnd.Execute("/wallet add otherSource 10");
            
            //when
            _endToEnd.Execute("/wallet balance");

            //then
            _endToEnd.AssertExpectedResult(
                "         source:  2.00",
                "    otherSource: 10.00",
                "               : 12.00"
                );
        }

        [Test]
        public void ShouldDisplayFullBalanceOfASingleTag()
        {
            //given
            _endToEnd.Execute("/wallet add source 6 'desc' #tag1 #ootherTag2 -date 2012-03-03");
            _endToEnd.Execute("/wallet add source 80 'desc' #tag2 #ootherTag5 -date 2012-03-03");
            _endToEnd.Execute("/wallet sub source 2 'desc' #tag1 #ootherTag4 -date 2012-04-03");
            _endToEnd.Execute("/wallet sub source 92 'desc' #tag2 #ootherTag3 -date 2012-04-03");

            //when
            _endToEnd.Execute("/wallet balance #tag1");

            //then
            _endToEnd.AssertExpectedResult(
                "    #tag1: 4.00");
        }

        [Test]
        public void ShouldDisplaySingleTagBalanceForMonth()
        {
            //given
            _endToEnd.Execute("/wallet add source 6 'desc' #tag1 #ootherTag2 -date 2012-03-03");
            _endToEnd.Execute("/wallet add source 80 'desc' #tag2 #ootherTag5 -date 2012-03-03");
            _endToEnd.Execute("/wallet sub source 2 'desc' #tag1 #ootherTag4 -date 2012-04-03");
            _endToEnd.Execute("/wallet sub source 92 'desc' #tag2 #ootherTag3 -date 2012-04-03");

            _endToEnd.SetTime(new DateTime(2012, 04, 03));

            //when
            _endToEnd.Execute("/wallet balance #tag1 --m");

            //then
            _endToEnd.AssertExpectedResult(
                "    #tag1: -2.00");
        }

        [Test]
        public void ShouldDisplaySingleTagBalanceForSpecifiedMonth()
        {
            //given
            _endToEnd.Execute("/wallet add source 6 'desc' #tag1 #ootherTag2 -date 2012-03-03");
            _endToEnd.Execute("/wallet add source 80 'desc' #tag2 #ootherTag5 -date 2012-03-03");
            _endToEnd.Execute("/wallet sub source 2 'desc' #tag1 #ootherTag4 -date 2012-04-03");
            _endToEnd.Execute("/wallet sub source 92 'desc' #tag2 #ootherTag3 -date 2012-04-03");

            _endToEnd.SetTime(new DateTime(2012, 04, 03));

            //when
            _endToEnd.Execute("/wallet balance #tag1 --m -month 2012-03");

            //then
            _endToEnd.AssertExpectedResult(
                "    #tag1: 6.00");
        }

        [Test]
        public void ShouldNotDisplayBalanceSumWhenDisplayingMultipleTagBalances()
        {
            //given
            _endToEnd.Execute("/wallet add source 6 'desc' #tag1 #ootherTag2 -date 2012-03-03");
            _endToEnd.Execute("/wallet add source 80 'desc' #tag2 #ootherTag5 -date 2012-03-03");
            _endToEnd.Execute("/wallet sub source 2 'desc' #tag1 #ootherTag4 -date 2012-04-03");
            _endToEnd.Execute("/wallet sub source 92 'desc' #tag2 #ootherTag3 -date 2012-04-03");
            
            _endToEnd.SetTime(new DateTime(2012, 04, 03));

            //when
            _endToEnd.Execute("/wallet balance #tag1 #ootherTag4 --m");

            //then
            _endToEnd.AssertExpectedResult(
                "          #tag1: -2.00",
                "    #ootherTag4: -2.00",
                "               :   [-]");
        }

        [Test]
        public void ShouldNotAllowMixingNormalSourcesAndTagsInBalanceDisplay()
        {
            //given
            _endToEnd.Execute("/wallet add source 6 'desc' #tag1 #ootherTag2 -date 2012-03-03");
            _endToEnd.Execute("/wallet add source 80 'desc' #tag2 #ootherTag5 -date 2012-03-03");
            _endToEnd.Execute("/wallet sub source 2 'desc' #tag1 #ootherTag4 -date 2012-04-03");
            _endToEnd.Execute("/wallet sub source 92 'desc' #tag2 #ootherTag3 -date 2012-04-03");

            _endToEnd.SetTime(new DateTime(2012, 04, 03));

            //when
            _endToEnd.Execute("/wallet balance #tag1 source");

            //then
            _endToEnd.AssertExpectedResult(
                "    Error: Cannot mix sources and tags in balance command.");
        }
    }
}