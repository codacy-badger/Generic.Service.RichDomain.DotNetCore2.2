# Generic Service - RichDomain, Asp.Net Core 2.2

## Initiative

This project has objective to made a CRUD more easily. 
Adding an extra layer of abstraction in application.
This project has building using the best programmation pratices.

Principles used:
* Reactive programmation;
* *DRY* principle;
* *SOLID* principles. 

This project is builded in *asp.net core 2.2* and has the dependencies below:
 * Microsoft.EntityFrameworkCore (>= 2.2.1)

#### *This project is focused in rich domains (well so I understood at least).*

Like or dislike, tell me and togheter make this project better.
*Come and be part of this project!*

Link to [this](https://www.nuget.org/packages/Generic.Service.DotNetCore/1.0.0) package on nuget.org.
Link to [Service](https://github.com/guilhermecaixeta/Generic.Service.RichDomain.DotNetCore2.2) 

## Version Notes

### VERSION 1.0.0 - Notes:
In this project you have a service layer to implements your projects;

## *DOCs*

For implements this package, follow the steps:
  
- In your Service make this:
  
```
public class MyEntity: BaseService<MyEntity, IBaseFilter>, IBaseService<MyEntity, IBaseFilter>
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

#### Attention this
For use auto generate lambda to filter is need make this:

```
//The entity is the same of above example.

//My Entity Filter
public class MyEntityFilter: IBaseFilter
{
  [LambdaGenerate(MethodOption = LambdaMethod.GreaterThanOrEqual, EntityPropertyName="Id")]
  public long CodeMin { get; set; }
  [LambdaGenerate(MethodOption = LambdaMethod.LessThanOrEqual, MergeOption= LambdaMerge.Or, EntityPropertyName="Id")]
  public long CodeMax { get; set; }
  /// The name property have the same name of entity property. Because this is not necessary set the EntityPropertyName. 
  [LambdaGenerate(MethodOption = LambdaMethod.Contains)]
  public string Name { get; set; }
}

//In Controller
public async Task<ActionResult<IEnumerable<MyEntity>>> GetFiltred([FromQuery]MyEntityFilter filter)
{
 return await _model.Filter(filter).ToListAsync();
}

//Lambda Generated
// x => x.Id >= valueId && x.Id <= valueId || x.Name.Contains(valueName)
```

To make a Page
```
JSON of BaseConfigurePage
{
  (int)page : 0,
  (int) size : 0
  (string) sort : "ASC"
  (string) order : "Id"
}

JSON Page format
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
        public Page<Category> GetPage([FromQuery]BaseConfigurePage config)
        {
            return _model.GetAll().ToPage<Entity>(config);
        }
...more code...
```

If you need map for another object

```
public class EntityReturn
{
  public string name {get; set;}
}

//....more code
public class MapData{

public class IEnumerable<EntityReturn> MapEntityToEntityReturn(IEnumerable<Entity> list){
 return list.Select(x => new EntityReturn{name = x.Name});
}

//....mode code

  [HttpGet("Paginate")]
  public Page<Entity, EntityReturn> GetPage([FromQuery]BaseConfigurePage config)
  {
      var funcMap = (Func<IEnumerable<Entity>, IEnumerable<EntityReturn>>)Delegate.CreateDelegate(typeof(Func<IEnumerable<Entity>, IEnumerable<EntityReturn>>), typeof(MapData).GetMethod("MapEntityToEntityReturn"));
      return _model.GetAll().ToPage<Entity, EntityReturn>(funcMap, config);
  }

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
