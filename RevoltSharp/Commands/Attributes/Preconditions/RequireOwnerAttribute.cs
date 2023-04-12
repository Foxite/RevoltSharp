using System;
using System.Threading.Tasks;

namespace RevoltSharp.Commands
{
    /// <summary>
    ///     Requires the command to be invoked by the owner of the bot.
    /// </summary>
    /// <remarks>
    ///     This precondition will restrict the access of the command or module to the owner of the Discord application.
    ///     If the precondition fails to be met, an erroneous <see cref="PreconditionResult"/> will be returned with the
    ///     message "Command can only be run by the owner of the bot."
    /// </remarks>
    /// <example>
    ///     The following example restricts the command to a set of sensitive commands that only the owner of the bot
    ///     application should be able to access.
    ///     <code language="cs">
    ///     [RequireOwner]
    ///     [Group("admin")]
    ///     public class AdminModule : ModuleBase
    ///     {
    ///         [Command("exit")]
    ///         public async Task ExitAsync()
    ///         {
    ///             Environment.Exit(0);
    ///         }
    ///     }
    ///     </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireOwnerAttribute : PreconditionAttribute
    {
        /// <inheritdoc />
        public override string ErrorMessage { get; set; }

        /// <inheritdoc />
        public override async Task<PreconditionResult> CheckPermissionsAsync(CommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Message.AuthorId == "01FE57SEGM0CBQD6Y7X10VZQ49")
                return PreconditionResult.FromSuccess();
            return PreconditionResult.FromError(ErrorMessage ?? "Command can only be run by the owner of the bot.");
        }
    }
}
