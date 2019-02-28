# Generic Model Layer

### VERSION 1.0.6 - Notes:
#### * All repositories names was changed, now the are BaseRepository and IBaseRepository.
### VERSION 1.0.9 - Notes:
#### * All Namespace of project was changed to make more easily and intuitive. 
 - BaseRepository
  * Before: GenericModel.Action
  * After: Generic.Repository.Base
 - Pagination
  * Before: GenericModel.Pagination
  * After: Generic.Repository.Extension.Pagination
 - BaseFilter
  * Before: GenericModel.Filter
  * After: Generic.Repository.Entity.Filter

#### * BaseFilter are changed to interface new is IBaseFilter.
#### * Filter was changed to attend more methods in lambda.

This project has objective to made a CRUD more easily. 

Adding an extra layer of abstraction in application.
This project has building using the best programmation pratices.

Principles used:
* Reactive progammation;
* SOLID principles. 

This project is builded in *asp.net core 2.2* and has the dependencies below:
 * Microsoft.EntityFrameworkCore (>= 2.2.1)

## * This project is focused in rich domains (well so I understood at least).*

Like or dislike, tell me and togheter make this project better.
*Come and be part of this project!*

Link to [this](https://www.nuget.org/packages/GenericModel/1.0.9) package on nuget.org.
Link to [repository](https://github.com/guilhermecaixeta/GenericModelLayer) 

## *DOCs*

For implements this package, follow the steps:

- Install package:
  * *Package Manager* > Install-Package GenericModel -Version 1.0.9
  * *.Net CLI* > dotnet add package GenericModel --version 1.0.9
  * *Paket CLI* > paket add GenericModel --version 1.0.9
  
- In your repository make this:
  
```
public class MyEntity: BaseRepository<MyEntity, IBaseFilter>, IBaseRepository<MyEntity, IBaseFilter>
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
### From version 1.0.9
#### Atention on this
* The filter property names need to be the same as the entity

Now this method can be generate lambda by names complements

If you add one this word below the generated lambda will attend this.

List words reserved to lambda methods:
* Equal
* Contains (only used in string types)
* GreaterThan
* LessThan
* GreaterThanOrEquals
* LessThanOrEquals

#### To use this words: IdEqual

Generated lambda: x => x.Id == value;

List words reserved to merge expressions:
* Or
* And

#### To use this words: IdEqualAnd

Generated lambda: x => x.Id == value && .....;

#### If none word reserved is informed on properties the method assumes the follow default values:
* word reserved to merge expressions : And
* word reserved to lambda methods: Equal

```
//The entity is the same of above example.

//My Entity Filter
public class MyEntityFilter: IBaseFilter
{
  [FromQuery(Name="Id")]
  public long IdEqualAnd {get; set;}
  [FromQuery(Name="Name")]
  public string NameContains {get; set;}
}

//In Controller
public async Task<ActionResult<IEnumerable<MyEntity>>> GetFiltred([FromQuery]MyEntityFilter filter)
{
 return await _model.Filter(filter).ToListAsync();
}

//Lambda Generated
// x => x.Id == valueId && x.Name.Contains(valueName)
```
### From version 1.0.6
To make a Pagination
```
JSON of BaseConfigurePagination
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
