SELECT [Date], [From], [Message], [IsDelay], [Hash] from [Log] WHERE [Date] BETWEEN ? AND ? AND [Jid] = ? ORDER BY [Id] AND [Type] = 'N'