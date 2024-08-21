using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pexita.Data;
using Pexita.Data.Entities.Tags;
using Pexita.Services.Interfaces;
using Pexita.Utility.Exceptions;

namespace Pexita.Services
{
    public class TagsService(AppDBContext Context, IMapper mapper) : ITagsService
    {
        private readonly AppDBContext _Context = Context;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Adds a new Tag to DB.
        /// </summary>
        /// <param name="tagTitle"></param>
        /// <returns></returns>
        public async Task AddTag(string tagTitle)
        {
            TagModel tag = new() { Title = tagTitle, TimesUsed = 0, Products = [] };
            _Context.Tags.Add(tag);
            await _Context.SaveChangesAsync();
        }
        /// <summary>
        /// Deletes a tag from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task DeleteTag(int id)
        {
            TagModel tag = await _Context.Tags.FindAsync(id) ?? throw new NotFoundException($"Tag {id} not found");
            _Context.Tags.Remove(tag);
            await _Context.SaveChangesAsync();
        }
        /// <summary>
        /// Gets a full list of tags from database.
        /// </summary>
        /// <returns>a List of <see cref="TagInfoDTO"/> objects representing DB records.</returns>
        public async Task<List<TagInfoDTO>> GetAllTags()
        {
            return await _Context.Tags.Include(x => x.Products)
                .AsNoTracking()
                .Select(x => _mapper.Map<TagInfoDTO>(x))
                .ToListAsync();
        }
        /// <summary>
        /// Gets a certain tag's info from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TagInfoDTO> GetTagByID(int id)
        {
            return _mapper.Map<TagInfoDTO>(await _Context.Tags.FindAsync(id));
        }
        /// <summary>
        /// converts tagmodel to DTO objects.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns> a <see cref="TagInfoDTO"/> object representing the DB record.</returns>
        public List<TagInfoDTO> TagsToDTO(List<TagModel> tags)
        {
            return tags.Select(x => new TagInfoDTO { ID = x.ID, Title = x.Title, TimesUsed = x.TimesUsed }).ToList();
        }
        /// <summary>
        /// checks if a tag exists by it's id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if exists false otherwise.</returns>
        public bool IsTag(int id)
        {
            return _Context.Tags.FirstOrDefault(t => t.ID == id) != null;
        }
        /// <summary>
        /// checks if a tag exists by it's title.
        /// </summary>
        /// <param name="TagName"></param>
        /// <returns>True if exists false otherwise.</returns>
        public bool IsTag(string TagName)
        {
            if (_Context.Tags.FirstOrDefault(t => t.Title == TagName) == null)
            {
                _Context.Tags.Add(new TagModel() { Title = TagName, Products = [] });
            }
            return true;
        }
    }
}
