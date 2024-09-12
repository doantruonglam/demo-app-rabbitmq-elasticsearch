import { BrowserRouter as Router, Routes, Route } from "react-router-dom"
import { CreateStudent } from "./features/students/create/create";
import { Student } from "./features/students/students";
import { UpdateStudent } from "./features/students/update/update";

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Student />} />
        <Route path="/create" element={<CreateStudent />} />
        <Route path="/update/:id" element={<UpdateStudent />} />
      </Routes>
    </Router>
  )
}

export default App
