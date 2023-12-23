namespace ModernUOGateway.Server;

public class GameServerListService
{
    private readonly Dictionary<Guid, GameServerListing> _gameServerListingById = new();
    private readonly SortedSet<GameServerListing> _gameServerListings = new(new RefreshComparer());
    
    // Todo: Use DTO instead of record
    public void RegisterGameServer(GameServerListing gameServerListing)
    {
        _gameServerListings.Add(gameServerListing);
        _gameServerListingById[gameServerListing.Id] = gameServerListing;
    }
    
    public void RefreshGameServer(Guid id)
    {
        if (!_gameServerListingById.TryGetValue(id, out var gameServerListing))
        {
            return;
        }

        gameServerListing.LastRefresh = DateTime.UtcNow;
        _gameServerListings.Remove(gameServerListing);
        _gameServerListings.Add(gameServerListing);
    }
    
    public bool UnregisterGameServer(Guid id)
    {
        return _gameServerListingById.Remove(id, out var gameServerListing) && _gameServerListings.Remove(gameServerListing);
    }
    
    public IEnumerable<GameServerListing> GetGameServerListings() => _gameServerListingById.Values;
}

internal class RefreshComparer : IComparer<GameServerListing>
{
    public int Compare(GameServerListing x, GameServerListing y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (ReferenceEquals(null, y))
        {
            return 1;
        }

        if (ReferenceEquals(null, x))
        {
            return -1;
        }

        return x.LastRefresh.CompareTo(y.LastRefresh);
    }
}