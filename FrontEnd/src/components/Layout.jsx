import { NavLink, Outlet } from "react-router-dom";
import { Activity, Dumbbell, Users, UserRound, CreditCard, Package, ClipboardList, BarChart3, Home, Info, Menu } from "lucide-react";
import { useState } from "react";

const links = [
  { to: "/", label: "Home", icon: Home },
  { to: "/dashboard", label: "Dashboard", icon: BarChart3 },
  { to: "/members", label: "Members", icon: Users },
  { to: "/trainers", label: "Trainers", icon: UserRound },
  { to: "/plans", label: "Plans", icon: ClipboardList },
  { to: "/programs", label: "Programs", icon: Dumbbell },
  { to: "/assignments", label: "Assignments", icon: Activity },
  { to: "/payments", label: "Payments", icon: CreditCard },
  { to: "/equipment", label: "Equipment", icon: Package },
  { to: "/reports", label: "Reports", icon: BarChart3 },
  { to: "/about", label: "About", icon: Info },
];

export default function Layout() {
  const [open, setOpen] = useState(false);
  return (
    <div className="min-h-screen bg-slate-50">
      <aside className={`${open ? "translate-x-0" : "-translate-x-full"} fixed inset-y-0 left-0 z-40 w-72 bg-slate-950 text-white transition lg:translate-x-0`}>
        <div className="flex h-20 items-center gap-3 border-b border-white/10 px-6">
          <div className="grid h-11 w-11 place-items-center rounded-2xl bg-cyan-500"><Dumbbell /></div>
          <div>
            <h1 className="text-lg font-black">Muscab Gym</h1>
            <p className="text-xs text-slate-300">Management System</p>
          </div>
        </div>
        <nav className="space-y-1 p-4">
          {links.map(({ to, label, icon: Icon }) => (
            <NavLink key={to} to={to} onClick={() => setOpen(false)} className={({ isActive }) => `flex items-center gap-3 rounded-xl px-4 py-3 text-sm font-semibold transition ${isActive ? "bg-cyan-500 text-white" : "text-slate-300 hover:bg-white/10 hover:text-white"}`}>
              <Icon size={18} /> {label}
            </NavLink>
          ))}
        </nav>
      </aside>
      {open && <div onClick={() => setOpen(false)} className="fixed inset-0 z-30 bg-black/40 lg:hidden" />}
      <main className="lg:pl-72">
        <header className="sticky top-0 z-20 flex h-16 items-center justify-between border-b border-slate-200 bg-white/90 px-4 backdrop-blur lg:px-8">
          <button className="btn-light lg:hidden" onClick={() => setOpen(true)}><Menu size={18} /></button>
          <div>
            <p className="text-xs font-bold uppercase tracking-widest text-cyan-700">React + Tailwind + C# API</p>
            <h2 className="font-black text-slate-900">Gym Management Website</h2>
          </div>
          <a className="btn-primary hidden sm:inline-flex" href="http://localhost:5092/swagger" target="_blank">Open Swagger</a>
        </header>
        <div className="p-4 lg:p-8"><Outlet /></div>
      </main>
    </div>
  );
}
