using api.Features.Cards.CreateCard;
using api.Features.Cards.DeleteCard;
using api.Features.Cards.GetAllCards;
using api.Features.Cards.GetCardById;
using api.Features.Cards.Shared;
using api.Features.Cards.UpdateCard;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/cards")]
[Produces("application/json")]
public class CardController(
    CreateCardUseCase createCardUseCase,
    DeleteCardUseCase deleteCardUseCase,
    GetAllCardsUseCase getAllCardsUseCase,
    GetCardByIdUseCase getCardByIdUseCase,
    UpdateCardUseCase updateCardUseCase) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCard(
        [FromBody] CreateCardRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("card.request.invalid", "Invalid data."));
        }

        var result = await createCardUseCase.ExecuteAsync(request, cancellationToken);

        return ToActionResult(
            result,
            response => CreatedAtAction(nameof(GetCardById), new { id = response.Id }, response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(CardResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCards(CancellationToken cancellationToken)
    {
        var cards = await getAllCardsUseCase.ExecuteAsync(cancellationToken);

        return Ok(cards);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCardById(Guid id, CancellationToken cancellationToken)
    {
        var result = await getCardByIdUseCase.ExecuteAsync(id, cancellationToken);

        return ToActionResult(result, Ok);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(CardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCard(
        Guid id,
        [FromBody] UpdateCardRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("card.request.invalid", "Invalid data."));
        }

        var result = await updateCardUseCase.ExecuteAsync(id, request, cancellationToken);

        return ToActionResult(result, Ok);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCard(Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteCardUseCase.ExecuteAsync(id, cancellationToken);

        return ToActionResult(result, NoContent);
    }
}
