namespace MTGRoyal.Services
{
    public class TemporaryStateService
    {
        private readonly object syncRoot = new();

        private BudgetState budgetState = new();
        private readonly List<AdvisorMessageState> advisorMessages = new();
        private PersonalDraftState personalDraft = new();

        public BudgetState GetBudgetState()
        {
            lock (syncRoot)
            {
                return new BudgetState
                {
                    Budget = budgetState.Budget,
                    Items = budgetState.Items
                        .Select(item => new BudgetItemState
                        {
                            Name = item.Name,
                            Price = item.Price,
                            Quantity = item.Quantity
                        })
                        .ToList()
                };
            }
        }

        public void SaveBudgetState(BudgetState state)
        {
            lock (syncRoot)
            {
                budgetState = new BudgetState
                {
                    Budget = state.Budget,
                    Items = state.Items
                        .Select(item => new BudgetItemState
                        {
                            Name = item.Name,
                            Price = item.Price,
                            Quantity = item.Quantity
                        })
                        .ToList()
                };
            }
        }

        public IReadOnlyList<AdvisorMessageState> GetAdvisorMessages()
        {
            lock (syncRoot)
            {
                return advisorMessages
                    .Select(message => new AdvisorMessageState
                    {
                        Role = message.Role,
                        Text = message.Text
                    })
                    .ToList();
            }
        }

        public void AddAdvisorExchange(string prompt, string response)
        {
            lock (syncRoot)
            {
                advisorMessages.Add(new AdvisorMessageState
                {
                    Role = "user",
                    Text = prompt
                });

                advisorMessages.Add(new AdvisorMessageState
                {
                    Role = "ai",
                    Text = response
                });
            }
        }

        public PersonalDraftState GetPersonalDraft()
        {
            lock (syncRoot)
            {
                return new PersonalDraftState
                {
                    Nombre = personalDraft.Nombre,
                    Precio = personalDraft.Precio,
                    RarezaId = personalDraft.RarezaId,
                    Tipo = personalDraft.Tipo,
                    Coleccion = personalDraft.Coleccion,
                    ImagenUrl = personalDraft.ImagenUrl,
                    ColorIds = personalDraft.ColorIds.ToArray()
                };
            }
        }

        public void SavePersonalDraft(PersonalDraftState draft)
        {
            lock (syncRoot)
            {
                personalDraft = new PersonalDraftState
                {
                    Nombre = draft.Nombre,
                    Precio = draft.Precio,
                    RarezaId = draft.RarezaId,
                    Tipo = draft.Tipo,
                    Coleccion = draft.Coleccion,
                    ImagenUrl = draft.ImagenUrl,
                    ColorIds = draft.ColorIds.ToArray()
                };
            }
        }

        public void ClearPersonalDraft()
        {
            lock (syncRoot)
            {
                personalDraft = new PersonalDraftState();
            }
        }
    }

    public class BudgetState
    {
        public decimal Budget { get; set; }

        public List<BudgetItemState> Items { get; set; } = [];
    }

    public class BudgetItemState
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }

    public class AdvisorMessageState
    {
        public string Role { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }

    public class PersonalDraftState
    {
        public string Nombre { get; set; } = string.Empty;

        public decimal Precio { get; set; }

        public int RarezaId { get; set; }

        public string Tipo { get; set; } = string.Empty;

        public string? Coleccion { get; set; }

        public string? ImagenUrl { get; set; }

        public int[] ColorIds { get; set; } = [];
    }
}
