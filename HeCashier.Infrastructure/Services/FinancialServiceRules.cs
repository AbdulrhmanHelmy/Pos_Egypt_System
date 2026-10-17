using HeCashier.Data;
using HeCashier.Models;
using Microsoft.EntityFrameworkCore;

namespace HeCashier.Infrastructure.Services;

internal static class FinancialServiceRules
{
    public static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "يجب أن يكون المبلغ أكبر من صفر.");
    }

    public static string RequiredName(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("الاسم مطلوب.", parameterName);
        var normalized = value.Trim();
        if (normalized.Length > 200)
            throw new ArgumentOutOfRangeException(parameterName, "الاسم أطول من الحد المسموح.");
        return normalized;
    }

    public static string RequiredText(string? value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("هذا الحقل مطلوب.", parameterName);
        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentOutOfRangeException(parameterName, "النص أطول من الحد المسموح.");
        return normalized;
    }

    public static AuditEntry CreateAudit(int userId, string operation, string entityType, int? entityId, string? details = null) => new()
    {
        UserId = userId,
        Operation = operation,
        EntityReference = entityId.HasValue ? $"{entityType}:{entityId.Value};{details}" : entityType,
        OccurredAtUtc = DateTime.UtcNow
    };

    public static FinancialTransaction CreateTransaction(
        FinancialTransactionType type,
        string transactionType,
        decimal amount,
        decimal balanceBefore,
        bool incoming,
        string description,
        string beneficiaryName,
        int userId,
        int? safeId = null,
        int? bankAccountId = null,
        int? shiftId = null,
        string? referenceEntityType = null,
        int? referenceEntityId = null)
    {
        return new FinancialTransaction
        {
            Type = type,
            TransactionType = transactionType,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = incoming ? balanceBefore + amount : balanceBefore - amount,
            OccurredAtUtc = DateTime.UtcNow,
            Description = description,
            DepositorOrBeneficiaryName = beneficiaryName,
            UserId = userId,
            SafeId = safeId,
            BankAccountId = bankAccountId,
            ShiftId = shiftId,
            ReferenceEntityType = referenceEntityType,
            ReferenceEntityId = referenceEntityId
        };
    }

    public static async Task<int?> GetOpenShiftIdAsync(HeCashierDbContext db, int? shiftId)
    {
        if (!shiftId.HasValue)
            return null;
        var shift = await db.Shifts.SingleOrDefaultAsync(x => x.Id == shiftId.Value && x.Status == ShiftStatus.Open)
            ?? throw new InvalidOperationException("الوردية غير موجودة أو مغلقة.");
        return shift.Id;
    }
}
