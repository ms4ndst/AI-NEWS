/**
 * Catppuccin reference card (React).
 * Tailwind variant — requires the ctp namespace from assets/tailwind.config.js.
 *
 * Demonstrates:
 *   - mantle surface for raised content over base background
 *   - text/subtext1 hierarchy
 *   - overlay1 border, surface0 hover
 *   - mauve primary action, sky info accent
 */

export function Card({ title, description, status = 'info', children }) {
  const statusColor = {
    success: 'text-ctp-green',
    warning: 'text-ctp-yellow',
    error:   'text-ctp-red',
    info:    'text-ctp-sky',
  }[status];

  return (
    <article
      className="
        bg-ctp-mantle
        border border-ctp-overlay1
        rounded-ctp-md
        shadow-ctp-md
        p-6
        transition-colors
        hover:border-ctp-mauve
      "
    >
      <header className="flex items-baseline justify-between mb-2">
        <h3 className="text-ctp-text text-lg font-semibold">{title}</h3>
        <span className={`text-xs font-medium uppercase tracking-wide ${statusColor}`}>
          {status}
        </span>
      </header>

      <p className="text-ctp-subtext1 text-sm leading-relaxed mb-4">
        {description}
      </p>

      {children && (
        <div className="pt-4 border-t border-ctp-surface0">
          {children}
        </div>
      )}
    </article>
  );
}

/* Usage:
   <Card
     title="Deployment ready"
     description="All checks passed. Build is ready to ship."
     status="success"
   >
     <button className="
       bg-ctp-mauve text-ctp-base
       px-4 py-2 rounded-ctp-md
       font-medium text-sm
       hover:brightness-110 active:brightness-95
       focus-visible:outline focus-visible:outline-2
       focus-visible:outline-ctp-lavender focus-visible:outline-offset-2
     ">
       Deploy
     </button>
   </Card>
*/
