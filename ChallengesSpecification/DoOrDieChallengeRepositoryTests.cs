namespace ChallengesSpecification
{
    using System;
    using System.Linq;
    using Modules.Challenges.Data;
    using Modules.Challenges.UI;
    using NUnit.Framework;

    public class DoOrDieChallengeRepositoryTests
    {
        [Test]
        public void ShouldLoadChallengeDaysForEveryDayIndefiniteChallenge()
        {
            //given
            const string description = "do stuff";
            var doOrDieChallenge = new DoOrDieChallengeDefinition
            {
                Name = "chalenge",
                Cycle = new[] {1},
                Definition = new[]
                {
                    new Challenge
                    {
                        Description = description
                    }
                }
            };

            var challengeRepo = new DoOrDieChallengeRepository(doOrDieChallenge);
            var today = DateTime.Now;

            //when
            var days = challengeRepo.GetLastDays(2, today);

            //then
            Assert.That(days.Count, Is.EqualTo(2));
            Assert.That(days.First().Day, Is.EqualTo(today.Subtract(TimeSpan.FromDays(1))));
            Assert.That(days.Last().Day, Is.EqualTo(today));
            Assert.That(days.All(day => day.Challenges.Count() == 1));
            Assert.That(days.All(day => day.Challenges.All(challenge => !challenge.Success && challenge.Description == description)));
        }

        [Test]
        public void ShouldReturnDaysBeforeChallengeStartAsEmpty()
        {
            //given
            var today = DateTime.Now;
            var doOrDieChallenge = new DoOrDieChallengeDefinition
            {
                Name = "chalenge",
                Cycle = new[] { 1 },
                Definition = new[] { new Challenge() },
                ChallengeStart = today.Subtract(TimeSpan.FromDays(1))
            };

            var challengeRepo = new DoOrDieChallengeRepository(doOrDieChallenge);

            //when
            var days = challengeRepo.GetLastDays(3, DateTime.Now);

            //then
            Assert.That(days.First().Challenges, Is.Empty);
        }

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