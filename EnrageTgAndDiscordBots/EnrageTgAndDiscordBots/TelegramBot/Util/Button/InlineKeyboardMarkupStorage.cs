using Telegram.Bot.Types.ReplyMarkups;

namespace EnrageTgBotILovePchel.Util.Button
{
    public class InlineKeyboardMarkupStorage
    {
        public static InlineKeyboardMarkup ChooseFindingMenu = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.FindTeammate.Name,
                    BotButtonsStorage.SearchTeammateMenu.FindTeammate.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.EditQuestionnaire.Name,
                    BotButtonsStorage.SearchTeammateMenu.EditQuestionnaire.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.DeleteQuestionnaire.Name,
                    BotButtonsStorage.SearchTeammateMenu.DeleteQuestionnaire.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Back.Name,
                    BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            }
        });

        public static InlineKeyboardMarkup QuestionnaireDeleteConfirmation = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Confirm.Name,
                    BotButtonsStorage.SearchTeammateMenu.Confirm.CallBackData),
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Back.Name,
                    BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            }
        });

        public static InlineKeyboardMarkup QuestionnaireCreateConfirmation = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Confirm.Name,
                    BotButtonsStorage.SearchTeammateMenu.Confirm.CallBackData),
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Back.Name,
                    BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            }
        });


        public static InlineKeyboardMarkup FindTeammateControlMenu = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.PreviousPlayer.Name,
                    BotButtonsStorage.SearchTeammateMenu.PreviousPlayer.CallBackData),
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.NextPlayer.Name,
                    BotButtonsStorage.SearchTeammateMenu.NextPlayer.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Back.Name,
                    BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            }
        });

        public static InlineKeyboardMarkup MainMenu = new(new[]
        {
            // new[]
            // {
            //     InlineKeyboardButton.WithCallbackData(BotButtonsStorage.MainMenu.WhenIsNextTournament.Name,
            //         BotButtonsStorage.MainMenu.WhenIsNextTournament.CallBackData)
            // },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.MainMenu.WhoWeAre.Name,
                    BotButtonsStorage.MainMenu.WhoWeAre.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.MainMenu.FindCommand.Name,
                    BotButtonsStorage.MainMenu.FindCommand.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.MainMenu.Rules.Name,
                    BotButtonsStorage.MainMenu.Rules.CallBackData)
            }
        });

        public static InlineKeyboardMarkup AdminMainMenu = new(new[]
        {
            // new[]
            // {
            //     InlineKeyboardButton.WithCallbackData(BotButtonsStorage.AdminMainMenu.ChangeTournamentData.Name,
            //         BotButtonsStorage.AdminMainMenu.ChangeTournamentData.CallBackData),
            // },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.MainMenu.WhoWeAre.Name,
                    BotButtonsStorage.MainMenu.WhoWeAre.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.MainMenu.FindCommand.Name,
                    BotButtonsStorage.MainMenu.FindCommand.CallBackData)
            },
            // new[]
            // {
            //     InlineKeyboardButton.WithCallbackData(BotButtonsStorage.AdminMainMenu.WhenIsNextTournament.Name,
            //         BotButtonsStorage.AdminMainMenu.WhenIsNextTournament.CallBackData)
            // },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.MainMenu.Rules.Name,
                    BotButtonsStorage.MainMenu.Rules.CallBackData)
            }
        });

        public static InlineKeyboardMarkup MenuWithBackButton = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Back.Name,
                    BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            }
        });

        public static InlineKeyboardMarkup SelectSearchFilter = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.RatingFilter.Name,
                    BotButtonsStorage.SearchTeammateMenu.RatingFilter.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.PosFilter.Name,
                    BotButtonsStorage.SearchTeammateMenu.PosFilter.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.NoFilter.Name,
                    BotButtonsStorage.SearchTeammateMenu.NoFilter.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Back.Name,
                    BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            }
        });

        public static InlineKeyboardMarkup RatingFilter = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.LowRating.Name,
                    BotButtonsStorage.SearchTeammateMenu.LowRating.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.MidRating.Name,
                    BotButtonsStorage.SearchTeammateMenu.MidRating.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.HighRating.Name,
                    BotButtonsStorage.SearchTeammateMenu.HighRating.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.SuperHighRating.Name,
                    BotButtonsStorage.SearchTeammateMenu.SuperHighRating.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Back.Name,
                    BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            }
        });
        
        public static InlineKeyboardMarkup PosFilter = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.FirstPos.Name,
                    BotButtonsStorage.SearchTeammateMenu.FirstPos.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.SecondPos.Name,
                    BotButtonsStorage.SearchTeammateMenu.SecondPos.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.ThirdPos.Name,
                    BotButtonsStorage.SearchTeammateMenu.ThirdPos.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.FourthPos.Name,
                    BotButtonsStorage.SearchTeammateMenu.FourthPos.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.FifthPos.Name,
                    BotButtonsStorage.SearchTeammateMenu.FifthPos.CallBackData)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(BotButtonsStorage.SearchTeammateMenu.Back.Name,
                    BotButtonsStorage.SearchTeammateMenu.Back.CallBackData)
            }
        });
    }
}