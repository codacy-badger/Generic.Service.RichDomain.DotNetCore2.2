# Generic Model Layer

This a pilot project with objective to made a CRUD more easily. Adding an extra layer of abstraction in application. 

This project is builded in *asp.net core 2.2* and has the dependencies below:
 *     Microsoft.EntityFrameworkCore (>= 2.2.1)

## *All methods is Async. This project is focused in rich domains (well so I understood at least).*

Like or dislike, tell me and togheter make this project better.
*Come and be part of this project!*

Link to [this](https://www.nuget.org/packages/GenericModel/1.0.0) package on nuget.org.


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
public async ActionResult GetFiltred(MyEntityFilter filter)
{
 return await _model.Filter(filter).ToListAsync();
}
```
