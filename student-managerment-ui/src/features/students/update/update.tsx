import { useEffect, useState } from "react"
import {
  useGetStudentByIdQuery,
  useUpdateStudentMutation,
} from "../studentsApiSlice"
import { useNavigate, useParams } from "react-router-dom"
import styles from "../students.module.css"

export const UpdateStudent = () => {
  const { id } = useParams<{ id: string }>()
  const { data: student, isLoading } = useGetStudentByIdQuery(Number(id))
  const [updateStudent] = useUpdateStudentMutation()
  const navigate = useNavigate()

  const [name, setName] = useState(student?.name || "")
  const [dob, setDob] = useState(student?.dob || "")
  const [address, setAddress] = useState(student?.address || "")
  const [className, setClassName] = useState(student?.class || "")

  useEffect(() => {
    if (student) {
      setName(student.name)
      setDob(student.dob.split("T")[0])
      setAddress(student.address)
      setClassName(student.class)
    }
  }, [student])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (student) {
      await updateStudent({ ...student, name, dob, address, class: className })
      navigate("/", { state: { refetch: true } })
    }
  }

  if (isLoading) return <div>Loading...</div>

  return (
    <div className={styles.formContainer}>
      <h1>Update Student</h1>
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
          Update Student
        </button>
        <button className={styles.buttonDelete} onClick={() => navigate("/")}>
          Cancel
        </button>
      </form>
    </div>
  )
}
