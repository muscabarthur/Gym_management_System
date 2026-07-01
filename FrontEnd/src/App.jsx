import { Routes, Route } from "react-router-dom";
import Layout from "./components/Layout.jsx";
import Home from "./pages/Home.jsx";
import Dashboard from "./pages/Dashboard.jsx";
import CrudPage from "./pages/CrudPage.jsx";
import Assignments from "./pages/Assignments.jsx";
import Reports from "./pages/Reports.jsx";
import About from "./pages/About.jsx";
import { membersApi, trainersApi, plansApi, paymentsApi, equipmentApi, workoutProgramsApi } from "./api/api.js";

const configs = {
  members: { title: "Members", api: membersApi, id: "memberID", search: true, fields: [
    { name: "fullName", label: "Full Name", required: true }, { name: "gender", label: "Gender", type: "select", options: ["Male", "Female"] }, { name: "phone", label: "Phone" }, { name: "address", label: "Address" }
  ], columns: ["memberID", "fullName", "gender", "phone", "address"] },
  trainers: { title: "Trainers", api: trainersApi, id: "trainerID", search: true, fields: [
    { name: "trainerName", label: "Trainer Name", required: true }, { name: "specialty", label: "Specialty" }, { name: "phone", label: "Phone" }
  ], columns: ["trainerID", "trainerName", "specialty", "phone"] },
  plans: { title: "Membership Plans", api: plansApi, id: "planID", fields: [
    { name: "planName", label: "Plan Name", required: true }, { name: "durationMonths", label: "Duration Months", type: "number", required: true }, { name: "price", label: "Price", type: "number", required: true }
  ], columns: ["planID", "planName", "durationMonths", "price"] },
  payments: { title: "Payments", api: paymentsApi, id: "paymentID", fields: [
    { name: "memberID", label: "Member ID", type: "number", required: true }, { name: "amount", label: "Amount", type: "number", required: true }, { name: "paymentDate", label: "Payment Date", type: "date", required: true }
  ], columns: ["paymentID", "memberID", "memberName", "amount", "paymentDate"] },
  equipment: { title: "Equipment", api: equipmentApi, id: "equipmentID", search: true, fields: [
    { name: "equipmentName", label: "Equipment Name", required: true }, { name: "quantity", label: "Quantity", type: "number", required: true }, { name: "status", label: "Status", type: "select", options: ["Available", "Maintenance", "Out of Stock"] }
  ], columns: ["equipmentID", "equipmentName", "quantity", "status"] },
  programs: { title: "Workout Programs", api: workoutProgramsApi, id: "programID", search: true, fields: [
    { name: "programName", label: "Program Name", required: true }, { name: "trainerID", label: "Trainer ID", type: "number", required: true }
  ], columns: ["programID", "programName", "trainerID", "trainerName", "specialty"] },
};

export default function App() {
  return <Routes><Route element={<Layout />}>
    <Route index element={<Home />} />
    <Route path="dashboard" element={<Dashboard />} />
    <Route path="members" element={<CrudPage config={configs.members} />} />
    <Route path="trainers" element={<CrudPage config={configs.trainers} />} />
    <Route path="plans" element={<CrudPage config={configs.plans} />} />
    <Route path="programs" element={<CrudPage config={configs.programs} />} />
    <Route path="assignments" element={<Assignments />} />
    <Route path="payments" element={<CrudPage config={configs.payments} />} />
    <Route path="equipment" element={<CrudPage config={configs.equipment} />} />
    <Route path="reports" element={<Reports />} />
    <Route path="about" element={<About />} />
  </Route></Routes>;
}
