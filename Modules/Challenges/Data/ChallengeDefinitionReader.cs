namespace Modules.Challenges.Data
{
    using System.IO;
    using Newtonsoft.Json;

    public class ChallengeDefinitionReader
    {
        private const string ChallengesPath = "_challenges";

        public static DoOrDieChallengeDefinition ReadDoOrDieDefinition(string challengeName)
        {
            var challengeFilePath = string.Format("{0}/{1}.json", ChallengesPath, challengeName);
            return
                JsonConvert.DeserializeObject<DoOrDieChallengeDefinition>(
                    File.ReadAllText(challengeFilePath));
        }
    }
}