namespace ChallengesSpecification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Modules.Challenges;
    using NUnit.Framework;

    public class DoOrDieChallengeTests
    {
        [Test]
        public void ShouldLoadChallengeDaysForEveryDayIndefiniteChallenge()
        {
            //given
            var doOrDieChallenge = new DoOrDieChallenge
            {
                Name = "chalenge"
            };

            var today = DateTime.Now;

            //when
            var days = doOrDieChallenge.GetLastDays(2, today);

            //then
            Assert.That(days.Count, Is.EqualTo(2));
            Assert.That(days.First().Day, Is.EqualTo(today.Subtract(TimeSpan.FromDays(1))));
            Assert.That(days.Last().Day, Is.EqualTo(today));
            Assert.That(days.All(day => !day.ChallengeResult.Success));
        }

        [Test]
        public void ShouldReturnDaysBeforeChallengeStartAsEmpty()
        {
            //given
            var today = DateTime.Now;
            var doOrDieChallenge = new DoOrDieChallenge
            {
                Name = "chalenge",
                ChallengeStart = today.Subtract(TimeSpan.FromDays(1))
            };

            //when
            var days = doOrDieChallenge.GetLastDays(3, DateTime.Now);

            //then
            Assert.That(days.First().ChallengeResult, Is.Null);
        }

        [Test]
        public void ShouldMarkADayAsDone()
        {
            //given
            var today = DateTime.Now;
            var doOrDieChallenge = new DoOrDieChallenge
            {
                Name = "chalenge",
                ChallengeStart = today.Subtract(TimeSpan.FromDays(1))
            };

            doOrDieChallenge.MarkAsDone(today);

            //when
            var days = doOrDieChallenge.GetLastDays(1, DateTime.Today);

            //then
            Assert.That(days.First().ChallengeResult.Success, Is.True);
        }
    }

    public class IDontKnowHowDotNetWorksTests
    {
        [Test]
        public void ShouldSubtractDaysFromDateTime()
        {
            //given
            var now = new DateTime(2015, 10, 5);

            //when
            var backThen = now.Subtract(TimeSpan.FromDays(5));

            //then
            Assert.That(backThen, Is.EqualTo(new DateTime(2015, 9, 30)));
        }

        [Test]
        public void ShouldStoreValuesByDateTime()
        {
            //given
            var dict = new Dictionary<DateTime, int>();

            var date = DateTime.Today;

            dict.Add(date.Date, 1);

            //when
            int result;
            dict.TryGetValue(date.Date, out result);

            //then
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void ResetsTheValueAtQuestionWhenTryingToGetValueThatIsNotInTheDictionary()
        {
            //given
            var dict = new Dictionary<DateTime, int>();

            const int expectedResult = 3;
            var result = expectedResult;

            //when
            dict.TryGetValue(DateTime.Today.Date, out result);

            //then
            Assert.That(result, Is.EqualTo(0));
        }
    }
}