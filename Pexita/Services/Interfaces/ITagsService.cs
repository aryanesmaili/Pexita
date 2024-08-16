using Pexita.Data.Entities.Tags;

namespace Pexita.Services.Interfaces
{
    public interface ITagsService
    {
        public Task AddTag(string tagTitle);
        public Task<List<TagInfoDTO>> GetAllTags();
        public Task<TagInfoDTO> GetTagByID(int id);
        public Task DeleteTag(int id);
        public List<TagInfoDTO> TagsToDTO(List<TagModel> tags);
        public bool IsTag(int id);
        public bool IsTag(string TagName);
    }
}
