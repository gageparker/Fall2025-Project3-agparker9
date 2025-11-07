using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Fall2025_Project3_agparker9.Services
{
    public class AiService
    {
        private readonly AzureOpenAIClient _client;
        private readonly string _deploymentName;

        public AiService(IConfiguration configuration)
        {
            
            var endpoint = new Uri(configuration["OpenAI:Endpoint"]!);
            var apiKey = configuration["OpenAI:Key"];
            _deploymentName = configuration["OpenAI:DeploymentName"]!;
            
            
           
            _client = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey!));
        }

        public async Task<List<string>> GenerateMovieReviewsAsync(string movieTitle, string genre, int year)
        {
            var chatClient = _client.GetChatClient(_deploymentName);

            var prompt = $@"Generate exactly 10 diverse movie reviews for the film '{movieTitle}' ({year}, {genre} genre). 
Each review should be 2-3 sentences long and represent different perspectives (positive, negative, mixed).
Format your response as a numbered list from 1-10, with each review on a new line.
Example format:
1. Review text here.
2. Another review text here.
Do not include any other text or explanation.";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant that generates realistic movie reviews."),
                new UserChatMessage(prompt)
            };

            var response = await chatClient.CompleteChatAsync(messages);
            var content = response.Value.Content[0].Text;

            return ParseReviews(content, 10);
        }

        public async Task<List<string>> GenerateActorTweetsAsync(string actorName, int age, string gender)
        {
            var chatClient = _client.GetChatClient(_deploymentName);

            var prompt = $@"Generate exactly 20 diverse tweets about the actor '{actorName}' (Age: {age}, Gender: {gender}).
The tweets should represent various perspectives: fans praising their work, critics discussing performances, news about projects, casual mentions, etc.
Each tweet should be realistic (under 280 characters) and varied in tone (positive, negative, neutral, excited, etc.).
Format your response as a numbered list from 1-20, with each tweet on a new line.
Example format:
1. Tweet text here.
2. Another tweet text here.
Do not include any other text, hashtags in numbering, or explanation.";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant that generates realistic social media content."),
                new UserChatMessage(prompt)
            };

            var response = await chatClient.CompleteChatAsync(messages);
            var content = response.Value.Content[0].Text;

            return ParseReviews(content, 20);
        }
    
        public async Task<List<string>> GetActorsInMovieAsync(string movieTitle)
        {
            var chatClient = _client.GetChatClient(_deploymentName);

            var prompt = $@"List all main actors in the movie '{movieTitle}'.
Format your response as a numbered list, one actor per line.
Do not include any additional explanation or text.";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant that lists actors in movies."),
                new UserChatMessage(prompt)
            };

            var response = await chatClient.CompleteChatAsync(messages);
            var content = response.Value.Content[0].Text;

            return ParseReviews(content, 50); 
        }

        public async Task<List<string>> GetMoviesForActorAsync(string actorName)
        {
            var chatClient = _client.GetChatClient(_deploymentName);

            var prompt = $@"List all movies that the actor '{actorName}' has appeared in.
Format your response as a numbered list, one movie per line.
Do not include any additional explanation or text.";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant that lists movies for actors."),
                new UserChatMessage(prompt)
            };

            var response = await chatClient.CompleteChatAsync(messages);
            var content = response.Value.Content[0].Text;

            return ParseReviews(content, 50); 
        }
        
        private List<string> ParseReviews(string content, int expectedCount)
        {
            var reviews = new List<string>();
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                
                var review = System.Text.RegularExpressions.Regex.Replace(trimmed, @"^\d+[\.\)\-\:]\s*", "");
                
                if (!string.IsNullOrWhiteSpace(review))
                {
                    reviews.Add(review.Trim());
                }

                if (reviews.Count >= expectedCount)
                    break;
            }

            return reviews;
        }
    }
}