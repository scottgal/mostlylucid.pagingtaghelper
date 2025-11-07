namespace mostlylucid.pagingtaghelper.sample.Services;

using System.Net.Http;
using System.Threading.Tasks;

public class GitHubCodeService(HttpClient httpClient)
{
    public async Task<string?> GetCodeFromGitHubAsync(string repoOwner, string repoName, string branch, string filePath)
    {
        try
        {
            // Construct raw GitHub URL
            var url = $"https://raw.githubusercontent.com/{repoOwner}/{repoName}/{branch}/{filePath}";

            // Fetch file content
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            return $"Error fetching code: {ex.Message}";
        }

        return "Error: File not found.";
    }
}