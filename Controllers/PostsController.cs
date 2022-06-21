using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using csharp_blog_backend.Models;

namespace csharp_blog_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly BlogContext _context;

        public PostsController(BlogContext context)
        {
            _context = context;

            
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> Getposts(string? stringa)
        {
          if (_context.posts == null)
          {
              return NotFound();
          }
            if (stringa != null) { return await _context.posts.Where(m => m.Title.Contains(stringa) || m.Description.Contains(stringa)).ToListAsync(); }

            else { return await _context.posts.ToListAsync(); }
        }




        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            if (_context.posts == null)
            {
                return NotFound();
            }
            var post = await _context.posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

           

            return post;
        }



        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Posts     questo controller funziona per la gestione del file
        // aggiunto per gestire il passaggio di un file
        
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost([FromForm] Post post)
        {
            FileInfo fileInfo = new FileInfo(post.File.FileName);
            //post.Image = $"FileLocal{fileInfo.Extension}"; // qwuesto è quello che viene salvato nel DB

            Guid g = Guid.NewGuid();

            string fileName = g.ToString()+ fileInfo.Extension;

                     

            //Estrazione File e salvataggio su file system.
            //Agendo su Request ci prendiamo il file e lo salviamo su file system.

            string Image = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files");

            if (!Directory.Exists(Image))
                Directory.CreateDirectory(Image);

            
            string fileNameWithPath = Path.Combine(Image, fileName);

                       
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                post.File.CopyTo(stream);
            }


            post.Image = "https://localhost:5000/Files/" + fileName;


            // salviamo anche il file come  varBinaryMAx nel DB
            //in questa parte c'è il salvataggio a db per un file blog

             byte[] b;

             //per leggerlo in html basta usare <img src="data:image/png;base64,iVBORw0KGgoAAAANSU ...">

            using (BinaryReader br = new BinaryReader(post.File.OpenReadStream()))

            { 
                post.ImageBytes = br.ReadBytes((int) post.File.OpenReadStream().Length);
            }



            _context.posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }



        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Questa parte di controller gestisce il salvataggio da stringa (senza file ) 

        //[HttpPost]
        //public async Task<ActionResult<Post>> PostPost(Post post)
        //{
        //    if (_context.posts == null)
        //    {
        //        return Problem("Entity set 'BlogContext.posts'  is null.");
        //    }
        //    _context.posts.Add(post);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetPost", new { id = post.Id }, post);
        //}

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (_context.posts == null)
            {
                return NotFound();
            }
            var post = await _context.posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return (_context.posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
