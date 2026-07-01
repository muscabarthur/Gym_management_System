import CrudPage from "./CrudPage.jsx";
import { memberProgramsApi } from "../api/api.js";
const config = { title: "Member Program Assignments", api: memberProgramsApi, id: "memberProgramID", search: true, fields: [
  { name: "memberID", label: "Member ID", type: "number", required: true },
  { name: "programID", label: "Program ID", type: "number", required: true },
  { name: "startDate", label: "Start Date", type: "date", required: true }
], columns: ["memberProgramID", "memberID", "memberName", "programID", "programName", "trainerName", "startDate"] };
export default function Assignments(){ return <CrudPage config={config}/>; }
