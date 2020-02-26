using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletApp.Database;
using Microsoft.EntityFrameworkCore;
using WalletApp.Models;
using WalletApp.Services.Currency;

namespace WalletApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        WalletContext _context;
        ICurrencyService _currencyService;
        public WalletController(WalletContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        [HttpGet("{userId}/getwallets")]
        public async Task<WalletApiResponse> GetWallets(int userId)
        {
            try
            {
                var wallets = await _context.Wallets.Where(x => x.UserId == userId).ToArrayAsync();
                return new GetWalletsResponse(wallets);
            }
            catch (Exception ex)
            {
                return new WalletApiErrorResponse(ex);
            }
        }

        [HttpGet("getcurrencies")]
        public WalletApiResponse GetCurrencies()
        {
            try
            {
                return new GetCurrenciesResponse(_currencyService.GetCurrencies());
            }
            catch (Exception ex)
            {
                return new WalletApiErrorResponse(ex);
            }
        }

        [HttpPost("{userId}/createwallet")]
        public async Task<WalletApiResponse> CreateWallet(int userId, [FromBody] CreateWalletRequest createWalletRequest)
        {
            try
            {
                if (_currencyService.HasCurrencyRate(createWalletRequest.Currency))
                {
                    var newWallet = new Wallet()
                    {
                        Currency = createWalletRequest.Currency,
                        UserId = userId
                    };
                    _context.Wallets.Add(newWallet);
                    await _context.SaveChangesAsync();

                    return new CreateWalletResponse(newWallet);
                }
                else
                {
                    return new WalletApiErrorResponse($"Нельзя создать кошелек с валютой {createWalletRequest.Currency}");
                }
            }
            catch (Exception ex)
            {
                return new WalletApiErrorResponse(ex);
            }
        }

        [HttpPost("{userId}/movement")]
        public async Task<WalletApiResponse> Movement(int userId, [FromBody] MovementRequest createWalletMovementRequest)
        {
            try
            {
                if (createWalletMovementRequest.Money == 0)
                {
                    return new WalletApiErrorResponse("Нельзя создать движение с нулевой суммой");
                }
                using (var trans = _context.Database.BeginTransaction())
                {
                    var wallet = _context.Wallets.FirstOrDefault(x => x.UserId == userId && x.Id == createWalletMovementRequest.WalletId);
                    if (wallet == null)
                    {
                        return new WalletApiErrorResponse($"Кошелька с ID = {createWalletMovementRequest.WalletId} не существует");
                    }
                    else
                    {
                        wallet.CacheSum += createWalletMovementRequest.Money;
                        if (wallet.CacheSum >= 0)
                        {
                            wallet.Operations.Add(new WalletOperation()
                            {
                                Mov = createWalletMovementRequest.Money,
                                CreateDate = DateTime.Now,
                                Comment = createWalletMovementRequest.Money > 0 ? "Пополнение" : "Снятие"
                            });

                            await _context.SaveChangesAsync();
                            await trans.CommitAsync();
                            return new CreateWalletResponse(wallet);
                        }
                        else
                        {
                            return new WalletApiErrorResponse("Не хватает денег");
                        }
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return new WalletApiErrorResponse("Ошибка операции, состояние кошелька было изменено, во время операции");
            }
            catch (Exception ex)
            {
                return new WalletApiErrorResponse(ex);
            }
        }

        [HttpPost("{userId}/transfer")]
        public async Task<WalletApiResponse> Transfer(int userId, [FromBody] TransferRequest convertMoneyRequest)
        {
            try
            {
                if (convertMoneyRequest.Sum <= 0)
                {
                    return new WalletApiErrorResponse("Нельзя создать перевод с суммой меньшей или равной нулю");
                }
                if(convertMoneyRequest.SourceWalletId == convertMoneyRequest.TargetWalletId)
                {
                    return new WalletApiErrorResponse("Нельзя создать перевод в этот же кошлек");
                }
                using (var trans = _context.Database.BeginTransaction())
                {
                    var sourceWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == convertMoneyRequest.SourceWalletId);
                    if (sourceWallet == null)
                    {
                        return new WalletApiErrorResponse($"Не существует кошелька с Id = {convertMoneyRequest.SourceWalletId}");
                    }

                    var targetWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == convertMoneyRequest.TargetWalletId);
                    if (targetWallet == null)
                    {
                        return new WalletApiErrorResponse($"Не существует кошелька с Id = {convertMoneyRequest.TargetWalletId}");
                    }

                    var convertedSum = _currencyService.Convert(convertMoneyRequest.Sum, sourceWallet.Currency, targetWallet.Currency);
                    var transactionDate = DateTime.Now;

                    sourceWallet.CacheSum -= convertMoneyRequest.Sum;
                    if(sourceWallet.CacheSum < 0)
                    {
                        return new WalletApiErrorResponse("Не хватает денег");
                    }

                    sourceWallet.Operations.Add(new WalletOperation()
                    {
                        Mov = -convertMoneyRequest.Sum,
                        CreateDate = transactionDate,
                        Comment = $"Перевод на кошелек {targetWallet.Id}"
                    });

                    targetWallet.CacheSum += convertedSum;
                    targetWallet.Operations.Add(new WalletOperation()
                    {
                        Mov = convertedSum,
                        CreateDate = transactionDate,
                        Comment = $"Перевод с кошелька {sourceWallet.Id}"
                    });

                    await _context.SaveChangesAsync();
                    await trans.CommitAsync();

                    return new TransferResponse(sourceWallet, targetWallet);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return new WalletApiErrorResponse("Ошибка операции, состояние кошелька было изменено, во время операции");
            }
            catch (Exception ex)
            {
                return new WalletApiErrorResponse(ex);
            }
        }
    }
}