import { useEffect, useMemo, useState } from "react";
import { Edit, Plus, RefreshCw, Search, Trash2, X } from "lucide-react";
import { getApiError } from "../api/api.js";

const emptyFrom = (fields) => Object.fromEntries(fields.map((f) => [f.name, f.type === "date" ? new Date().toISOString().slice(0, 10) : ""]));
const display = (value) => value == null ? "-" : String(value).includes("T00:00:00") ? String(value).slice(0, 10) : String(value);

export default function CrudPage({ config }) {
  const [rows, setRows] = useState([]);
  const [form, setForm] = useState(emptyFrom(config.fields));
  const [editing, setEditing] = useState(null);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const load = async () => {
    try {
      setLoading(true); setError("");
      const params = config.search && search ? { search } : {};
      setRows(await config.api.getAll(params));
    } catch (e) { setError(getApiError(e)); }
    finally { setLoading(false); }
  };
  useEffect(() => { load(); }, [config.title]);

  const title = useMemo(() => editing ? `Update ${config.title}` : `Add ${config.title}`, [editing, config.title]);
  const reset = () => { setEditing(null); setForm(emptyFrom(config.fields)); setError(""); };
  const startEdit = (row) => {
    const next = {};
    config.fields.forEach((f) => { next[f.name] = f.type === "date" && row[f.name] ? String(row[f.name]).slice(0,10) : row[f.name] ?? ""; });
    setEditing(row[config.id]); setForm(next); window.scrollTo({ top: 0, behavior: "smooth" });
  };
  const submit = async (e) => {
    e.preventDefault();
    try {
      setSaving(true); setError("");
      const payload = { ...form };
      config.fields.forEach((f) => { if (f.type === "number") payload[f.name] = Number(payload[f.name]); });
      if (editing) await config.api.update(editing, payload); else await config.api.create(payload);
      reset(); await load();
    } catch (e) { setError(getApiError(e)); }
    finally { setSaving(false); }
  };
  const remove = async (id) => {
    if (!confirm("Delete this record?")) return;
    try { setError(""); await config.api.remove(id); await load(); }
    catch (e) { setError(getApiError(e)); }
  };

  return <div className="space-y-6">
    <div className="flex flex-col justify-between gap-4 md:flex-row md:items-end">
      <div><p className="text-sm font-bold text-cyan-700">Management</p><h1 className="text-3xl font-black">{config.title}</h1></div>
      {config.search && <form className="flex gap-2" onSubmit={(e)=>{e.preventDefault(); load();}}><div className="relative"><Search className="absolute left-3 top-3 text-slate-400" size={18}/><input className="input pl-10" value={search} onChange={(e)=>setSearch(e.target.value)} placeholder="Search..." /></div><button className="btn-primary">Search</button><button type="button" onClick={()=>{setSearch(""); setTimeout(load, 0)}} className="btn-light"><RefreshCw size={17}/></button></form>}
    </div>
    {error && <div className="rounded-xl border border-rose-200 bg-rose-50 p-4 font-semibold text-rose-700">{error}</div>}
    <form onSubmit={submit} className="card p-5">
      <div className="mb-4 flex items-center justify-between"><h2 className="text-xl font-black">{title}</h2>{editing && <button type="button" className="btn-light" onClick={reset}><X size={17}/>Cancel</button>}</div>
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {config.fields.map((field) => <label key={field.name} className="block"><span className="mb-1 block text-sm font-bold text-slate-700">{field.label}</span>{field.type === "select" ? <select className="input" required={field.required} value={form[field.name]} onChange={(e)=>setForm({...form,[field.name]:e.target.value})}><option value="">Choose</option>{field.options.map((o)=><option key={o} value={o}>{o}</option>)}</select> : <input className="input" type={field.type || "text"} required={field.required} value={form[field.name]} onChange={(e)=>setForm({...form,[field.name]:e.target.value})}/>}</label>)}
      </div>
      <button disabled={saving} className="btn-primary mt-5"><Plus size={17}/>{saving ? "Saving..." : editing ? "Update" : "Save"}</button>
    </form>
    <div className="card overflow-hidden">
      <div className="border-b border-slate-200 p-5"><h2 className="text-xl font-black">Records</h2></div>
      <div className="overflow-x-auto"><table className="w-full min-w-[760px] text-left text-sm"><thead className="bg-slate-100 text-xs uppercase text-slate-500"><tr>{config.columns.map((c)=><th key={c} className="px-5 py-3">{c}</th>)}<th className="px-5 py-3 text-right">Actions</th></tr></thead><tbody className="divide-y divide-slate-100">{loading ? <tr><td className="px-5 py-5" colSpan={config.columns.length+1}>Loading...</td></tr> : rows.length === 0 ? <tr><td className="px-5 py-5" colSpan={config.columns.length+1}>No records found.</td></tr> : rows.map((row)=><tr key={row[config.id]} className="hover:bg-slate-50">{config.columns.map((c)=><td key={c} className="px-5 py-4 font-medium text-slate-700">{display(row[c])}</td>)}<td className="px-5 py-4"><div className="flex justify-end gap-2"><button onClick={()=>startEdit(row)} className="btn-light"><Edit size={16}/></button><button onClick={()=>remove(row[config.id])} className="btn-danger"><Trash2 size={16}/></button></div></td></tr>)}</tbody></table></div>
    </div>
  </div>;
}
