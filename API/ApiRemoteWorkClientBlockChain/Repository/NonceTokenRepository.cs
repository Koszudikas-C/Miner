using System.Net;
using ApiRemoteWorkClientBlockChain.Data;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using LibCommunicationStateRemote.Entities;
using LibRemoteAndClient.Entities.Remote.Client;
using Microsoft.EntityFrameworkCore;

namespace ApiRemoteWorkClientBlockChain.Repository;

public class NonceTokenRepository(RemoteWorkClientDbContext dbContext,
    ILogger<NonceTokenRepository> logger) : INonceToken
{
    public ApiResponse<bool> Add(GuidTokenAuth entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<bool> Update(GuidTokenAuth entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<bool> Delete(GuidTokenAuth entity)
    {
        throw new NotImplementedException();
    }
    public ApiResponse<GuidTokenAuth> GetById(int id)
    {
        if(id == 0)
            return new ApiResponse<GuidTokenAuth>(HttpStatusCode.NotFound,
                false, "The supplier identifier cannot be zero!");

        try
        {
            var nonce = dbContext.GuidTokenAuths.Find(id);
            
            if (nonce is null)
                return new ApiResponse<GuidTokenAuth>(HttpStatusCode.NotFound,
                    false, "No subject was found");

            return new ApiResponse<GuidTokenAuth>(HttpStatusCode.OK, true,
                "Given caught successfully!", [nonce]);
        }
        catch (Exception e)
        {
            logger.LogError($"An error occurred when getting the nonce token from the database. Error: {e.Message}");
            throw new Exception();
        }
    }
    public ApiResponse<List<GuidTokenAuth>> GetAll(int page, int pageSize, bool all)
    {
        try
        {
            var nonce = dbContext.GuidTokenAuths.AsNoTracking().Skip(page - 1)
                .Take(pageSize).ToList();

            if (!nonce.Any())
                return new ApiResponse<List<GuidTokenAuth>>(HttpStatusCode.NotFound, false,
                    "No data was found");

            return new ApiResponse<List<GuidTokenAuth>>(HttpStatusCode.OK, true,
                "", [nonce]);
        }
        catch (Exception e)
        {
            logger.LogError($"An error occurred when getting all the nonce token from the database. Error: {e.Message}");
            throw new Exception();
        }
    }

    public ApiResponse<GuidTokenAuth> GetByNonce(Guid nonce)
    {
        if(nonce == Guid.Empty)
            return new ApiResponse<GuidTokenAuth>(HttpStatusCode.NotFound,
                false, "The supplier identifier cannot be zero!");

        try
        {
            var nonceFind = dbContext.GuidTokenAuths.Find(nonce);
            
            if (nonceFind is null)
                return new ApiResponse<GuidTokenAuth>(HttpStatusCode.NotFound,
                    false, "No subject was found");

            return new ApiResponse<GuidTokenAuth>(HttpStatusCode.OK, true,
                "Given caught successfully!", [nonceFind]);
        }
        catch (Exception e)
        {
            logger.LogError($"An error occurred when getting the nonce token from the database. Error: {e.Message}");
            throw new Exception();
        }
    }
    public Task<ApiResponse<bool>> AddAsync(GuidTokenAuth entity, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> UpdateAsync(GuidTokenAuth entity, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> DeleteAsync(GuidTokenAuth entity, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<GuidTokenAuth>> GetByIdAsync(int id, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<IEnumerable<GuidTokenAuth>>> GetAllAsync(int page, int pageSize,
        CancellationToken cts = default)
    {
        try
        {
            var nonce = await dbContext.GuidTokenAuths.AsNoTracking().Skip(page - 1)
                .Take(pageSize).ToListAsync(cts);

            if (!nonce.Any())
                return new ApiResponse<IEnumerable<GuidTokenAuth>>(HttpStatusCode.NotFound, false,
                    "No data was found");

            return new ApiResponse<IEnumerable<GuidTokenAuth>>(HttpStatusCode.OK, true,
                "", [nonce]);
        }
        catch (Exception e)
        {
            logger.LogError($"An error occurred when getting all the nonce token from the database. Error: {e.Message}");
            throw new Exception();
        }
    }
}