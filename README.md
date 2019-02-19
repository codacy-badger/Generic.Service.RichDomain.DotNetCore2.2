# Generic Model Layer

This a pilot project with objective to made a CRUD more easily. Adding an extra layer of abstraction in application. 

This project is builded in *asp.net core 2.2* and has the dependencies below:
 * Microsoft.EntityFrameworkCore (>= 2.2.1)

## *All methods is Async. This project is focused in rich domains (well so I understood at least).*

Like or dislike, tell me and togheter make this project better.
*Come and be part of this project!*

Link to [this](https://www.nuget.org/packages/GenericModel/1.0.0) package on nuget.org.
Link to [repository](https://github.com/guilhermecaixeta/GenericModelLayer) 

## *DOCs*

For implements this package, follow the steps:

- Install package:
  * *Package Manager* > Install-Package GenericModel -Version 1.0.0
  * *.Net CLI* > dotnet add package GenericModel --version 1.0.0 
  * *Paket CLI* > paket add GenericModel --version 1.0.0 
  
- In your repository make this:
  
```
public class MyEntity: BaseAction<MyEntity, BaseFilter, MyContext>, IBaseAction<MyEntity, BaseFilter>
{
//if has any code you implements here!!!
}

///On the Controller
...Controller code
private readonly MyEntity _model;

...ctor

pulic async MyEntity GetById(long id)
{
  return await _model.GetByIdAsync(id);
}
....Controller code...
```

If you want filter method implements like this way:

```
//The entity is the same of above example.

//My Entity Filter
public class MyEntityFilter: BaseFilter
{
 public long Id {get; set;}
 public string Name {get; set;}
}

//My Entity
public IQueryable<MyEntity> Filter(MyEntityFilter filter)
{
 return GetAllBy(x => x.Id == filter.Id || x.name.Contains(filter.Name));
}

//In Controller
public async Task<ActionResult<IEnumerable<MyEntity>>> GetFiltred(MyEntityFilter filter)
{
 return await _model.Filter(filter).ToListAsync();
}
```

To make a Pagination
```
JSON of BaseConfigurePagination
{
  (int)page : 0,
  (int) size : 0
  (string) sort : "ASC"
  (string) order : "Id"
}

JSON Returned
{
  "content": [
    {
      Entity Array
    }
  ],
  "totalElements": 0,
  "sort": "string",
  "order": "string",
  "size": 0,
  "page": 0
}
...Controller Code
        [HttpGet("Paginate")]
        public Pagination<Category> GetPage([FromQuery]BaseConfigurePagination config)
        {
            return _model.GetAll().PaginateTo(config);
        }
...more code...
```

Saving data on database:
```
//The entity is the same of first example.

//In Controller
... Controller code

private readonly MyEntity _model;

//...ctor and more code.....

public async Task<ActionResult<MyEntity>> PostAsync(MyEntity entity)
{
  _model.Map(entity);
  entity = await _model.CreateAsync();
 return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, entity);
}
```

Updating data:
```
//.....more code....

public async Task<ActionResult> PutAsync(long id, MyEntity entity)
{
  if(id != entity.Id)
    return BadRequest();
  _model.Map(entity);
  await _model.UpdateAsync();
 return NoContent();
}
```

Delete data:
```
//...more code and finally...

public async Task<ActionResult> DeleteAsync(long id)
        {
            if (id < 1)
                return BadRequest();
            await _model.DeleteAsync(id);
            return NoContent();
        }
```

[Here](https://github.com/guilhermecaixeta/TodoApi) are the implemented package on project using web api.

Doubts or recommendations? 
Send me an e-mail: guilherme.lpcaixeta@gmail.com
