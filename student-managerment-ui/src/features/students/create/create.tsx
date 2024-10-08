import { useState } from "react"
import { useAddStudentMutation } from "../studentsApiSlice"
import { useNavigate } from "react-router-dom"
import styles from "../students.module.css"

export const CreateStudent = () => {
  const [name, setName] = useState("")
  const [dob, setDob] = useState("")
  const [address, setAddress] = useState("")
  const [className, setClassName] = useState("")
  const [addStudent] = useAddStudentMutation()
  const navigate = useNavigate()

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    await addStudent({ name, dob, address, class: className })
    navigate("/", { state: { refetch: true } })
  }

  return (
    <div className={styles.formContainer}>
      <h1>Create Student</h1>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Name"
          value={name}
          onChange={e => setName(e.target.value)}
          required
        />
        <input
          type="date"
          placeholder="Date of Birth"
          value={dob}
          onChange={e => setDob(e.target.value)}
          required
        />
        <input
          type="text"
          placeholder="Address"
          value={address}
          onChange={e => setAddress(e.target.value)}
          required
        />
        <input
          type="text"
          placeholder="Class"
          value={className}
          onChange={e => setClassName(e.target.value)}
          required
        />
        <button className={styles.buttonUpdate} type="submit">
          Add Student
        </button>
        <button className={styles.buttonDelete} onClick={() => navigate("/")}>
          Cancel
        </button>
      </form>
    </div>
  )
}
