namespace Identity.Domain;

public sealed record RefreshToken(
    string TokenHash,
    DateTime ExpiryUtc);
