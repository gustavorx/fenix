using api.Features.People.CreatePerson;
using api.Features.People.DeletePerson;
using api.Features.People.GetAllPeople;
using api.Features.People.GetPersonById;
using api.Features.People.UpdatePerson;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/people")]
public class PersonController(
    CreatePersonUseCase createPersonUseCase,
    DeletePersonUseCase deletePersonUseCase,
    GetAllPeopleUseCase getAllPeopleUseCase,
    GetPersonByIdUseCase getPersonByIdUseCase,
    UpdatePersonUseCase updatePersonUseCase) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePerson(
        [FromBody] CreatePersonRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("person.request.invalid", "Invalid data."));
        }

        var result = await createPersonUseCase.ExecuteAsync(request, cancellationToken);

        return ToActionResult(
            result,
            response => CreatedAtAction(nameof(GetPersonById), new { id = response.Id }, response));
    }

    [HttpGet]
    public async Task<IActionResult> GetPeople(CancellationToken cancellationToken)
    {
        var people = await getAllPeopleUseCase.ExecuteAsync(cancellationToken);

        return Ok(people);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPersonById(Guid id, CancellationToken cancellationToken)
    {
        var result = await getPersonByIdUseCase.ExecuteAsync(id, cancellationToken);

        return ToActionResult(result, Ok);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdatePerson(
        Guid id,
        [FromBody] UpdatePersonRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("person.request.invalid", "Invalid data."));
        }

        var result = await updatePersonUseCase.ExecuteAsync(id, request, cancellationToken);

        return ToActionResult(result, Ok);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePerson(Guid id, CancellationToken cancellationToken)
    {
        var result = await deletePersonUseCase.ExecuteAsync(id, cancellationToken);

        return ToActionResult(result, NoContent);
    }
}
