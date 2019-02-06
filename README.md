### Generic Model Layer

This a pilot project with objective to made a CRUD more easily. Adding an extra layer of abstraction in application. 

This project is builded in *asp.net core 2.2* and has the dependencies below:
 *     Microsoft.EntityFrameworkCore (>= 2.2.1)

## *All methods is Async.*

Link to [this](https://www.nuget.org/packages/GenericModel/1.0.0) package on nuget.org.


## DOCs

For implements this package, follow the steps:

- Install package:
  * *Package Manager* > Install-Package GenericModel -Version 1.0.0
  * *.Net CLI* > dotnet add package GenericModel --version 1.0.0 
  * *Paket CLI* > paket add GenericModel --version 1.0.0 
  
- In your repository make this:
  
```
public class MyEntityRepo: BaseAction<MyEntity, BaseFilter, MyContext>, IBaseAction<MyEntity, BaseFilter>
{
//if has any code you implements here!!!
}

///On the Controller
...Controller code
pulic async MyEntity GetById(long id)
{
  return await _myRepo.GetByIdAsync(id);
}
....Controller code...
```
