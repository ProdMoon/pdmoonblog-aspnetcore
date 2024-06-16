using Microsoft.AspNetCore.Mvc;
using PdmoonblogApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PdmoonblogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly BlogDbContext _db;

    public ArticleController(BlogDbContext db)
    {
        _db = db;
    }

    // GET: api/Article?skip=0&take=10
    [HttpGet]
    public IActionResult GetArticles(int skip = 0, int take = 10)
    {
        var articles = _db.Articles.Skip(skip).Take(take).ToList();

        return Ok(articles);
    }

    // GET: api/Article/{id}
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var article = _db.Articles.Find(id);
        if (article == null)
        {
            return NotFound();
        }

        return Ok(article);
    }

    // POST: api/Aritcle
    [HttpPost]
    public IActionResult Post([FromBody] Article article)
    {
        _db.Articles.Add(article);
        _db.SaveChanges();

        return CreatedAtAction(nameof(Get), new { id = article.Id }, article);
    }

    // PUT: api/Article/{id}
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] Article article)
    {
        if (id != article.Id)
        {
            return BadRequest();
        }

        article.UpdatedAt = DateTime.Now;
        _db.Entry(article).State = EntityState.Modified;
        _db.SaveChanges();

        return NoContent();
    }

    // DELETE: api/Article/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var article = _db.Articles.Find(id);
        if (article == null)
        {
            return NotFound();
        }

        _db.Articles.Remove(article);
        _db.SaveChanges();

        return NoContent();
    }
}
