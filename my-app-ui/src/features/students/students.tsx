import { useEffect, useState } from "react"
import {
  useDeleteStudentMutation,
  useGetAllElasticStudentsQuery,
} from "./studentsApiSlice"
import { Link, useLocation, useNavigate } from "react-router-dom"

export const Student = () => {
  const [pagination, setPagination] = useState({ pageNum: 1, pageSize: 10 })
  const location = useLocation()
  const navigate = useNavigate()
  const {
    data: students,
    isLoading,
    error,
    refetch,
  } = useGetAllElasticStudentsQuery(pagination)
  const [deleteStudent] = useDeleteStudentMutation()

  useEffect(() => {
    // Check if the location state contains the refetch flag
    if (location.state && location.state.refetch) {
      refetch()
      navigate(location.pathname, { replace: true })
    }
  }, [location.state, refetch, navigate])

  const handleDelete = async (id?: number) => {
    if (id != null) await deleteStudent(id)
  }

  const handleNextPage = () => {
    setPagination(prev => ({ ...prev, pageNum: prev.pageNum + 1 }))
  }

  const handlePreviousPage = () => {
    setPagination(prev => ({ ...prev, pageNum: Math.max(prev.pageNum - 1, 1) }))
  }

  if (isLoading) return <div>Loading...</div>
  if (error) return <div>Error: {JSON.stringify(error)}</div>

  return (
    <div>
      <h1>Students List</h1>
      <Link to="/create">
        <button>Create Student</button>
      </Link>

      <span>
        Viewing {students?.students.length} of {students?.totalStudent}{" "}
        students.
      </span>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>DOB</th>
            <th>Address</th>
            <th>Class</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {students?.students.map(student => (
            <tr key={student.id}>
              <td>{student.id}</td>
              <td>{student.name}</td>
              <td>{student.dob}</td>
              <td>{student.address}</td>
              <td>{student.class}</td>
              <td>
                <Link to={`/update/${student.id}`}>
                  <button>Update</button>
                </Link>
                <button onClick={() => handleDelete(student.id)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      <div>
        <button
          onClick={handlePreviousPage}
          disabled={pagination.pageNum === 1}
        >
          Previous
        </button>
        <button
          onClick={handleNextPage}
          disabled={students?.students.length === 0}
        >
          Next
        </button>
      </div>
    </div>
  )
}
