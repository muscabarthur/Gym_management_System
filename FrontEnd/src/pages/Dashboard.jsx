import { useEffect, useState } from "react";
import { CreditCard, Dumbbell, Package, Users, UserRound, ClipboardList } from "lucide-react";
import { dashboardApi, getApiError } from "../api/api.js";
const money = (n) => `$${Number(n || 0).toLocaleString()}`;
export default function Dashboard(){
  const [data,setData]=useState(null),[error,setError]=useState("");
  useEffect(()=>{dashboardApi.get().then(setData).catch(e=>setError(getApiError(e)))},[]);
  const stats = data ? [
    ["Members", data.totalMembers, Users], ["Trainers", data.totalTrainers, UserRound], ["Plans", data.totalPlans, ClipboardList], ["Programs", data.totalPrograms, Dumbbell], ["Equipment", data.totalEquipmentItems, Package], ["Total Payments", money(data.totalPayments), CreditCard]
  ] : [];
  return <div className="space-y-6"><div><p className="text-sm font-bold text-cyan-700">Overview</p><h1 className="text-3xl font-black">Dashboard</h1></div>{error && <div className="rounded-xl bg-rose-50 p-4 font-semibold text-rose-700">{error}</div>}{!data ? <div className="card p-8">Loading dashboard...</div> : <><div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">{stats.map(([label,value,Icon])=><div className="card p-6" key={label}><div className="mb-4 flex h-12 w-12 items-center justify-center rounded-2xl bg-cyan-50 text-cyan-700"><Icon/></div><p className="text-sm font-bold text-slate-500">{label}</p><h2 className="mt-1 text-3xl font-black">{value}</h2></div>)}</div><div className="grid gap-6 xl:grid-cols-2"><div className="card p-5"><h2 className="mb-4 text-xl font-black">Recent Members</h2>{data.recentMembers?.map(m=><div key={m.memberID} className="mb-3 rounded-xl bg-slate-50 p-4"><b>{m.fullName}</b><p className="text-sm text-slate-500">{m.phone || "No phone"} · {m.address || "No address"}</p></div>)}</div><div className="card p-5"><h2 className="mb-4 text-xl font-black">Recent Payments</h2>{data.recentPayments?.map(p=><div key={p.paymentID} className="mb-3 flex justify-between rounded-xl bg-slate-50 p-4"><span><b>{p.memberName}</b><p className="text-sm text-slate-500">{String(p.paymentDate).slice(0,10)}</p></span><b>{money(p.amount)}</b></div>)}</div></div></>}</div>
}
