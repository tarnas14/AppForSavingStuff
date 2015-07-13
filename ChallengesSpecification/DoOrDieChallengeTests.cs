namespace ChallengesSpecification
{
    using System;
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
    }
}