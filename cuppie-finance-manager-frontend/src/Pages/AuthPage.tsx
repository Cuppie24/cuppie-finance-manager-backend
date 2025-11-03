import React, { useState, useEffect } from 'react'
import { Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { loginSchema, registerSchema, type LoginFormData, type RegisterFormData } from '../lib/validation'
import { z } from 'zod'
import { AxiosError } from 'axios'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Card, CardContent } from '@/components/ui/card'
import { Alert, AlertDescription } from '@/components/ui/alert'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Skeleton } from '@/components/ui/skeleton'
import { AlertCircle, CheckCircle2, Loader2, User as UserIcon } from 'lucide-react'

const AuthPage: React.FC = () => {
  const [isLogin, setIsLogin] = useState(true)
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: ''
  })
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')

  const { login, register, isAuthenticated, isLoading } = useAuth()
  const navigate = useNavigate()

  useEffect(() => {
    if (!isLoading && isAuthenticated) {
      if (successMessage) {
        const timer = setTimeout(() => {
          navigate('/home', { replace: true })
        }, 1500)
        return () => clearTimeout(timer)
      } else {
        navigate('/home', { replace: true })
      }
    }
  }, [isLoading, isAuthenticated, navigate, successMessage])

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-50">
        <Card className="w-full max-w-md">
          <CardContent className="pt-6">
            <div className="flex flex-col items-center justify-center py-8">
              <Loader2 className="h-12 w-12 animate-spin text-indigo-600 mb-4" />
              <Skeleton className="h-4 w-32 mb-2" />
              <Skeleton className="h-4 w-24" />
            </div>
          </CardContent>
        </Card>
      </div>
    )
  }

  if (isAuthenticated && !successMessage) {
    return <Navigate to="/home" replace />
  }

  const getErrorMessage = (error: any): string => {
    if (error instanceof AxiosError) {
      const status = error.response?.status
      const serverMessage = error.response?.data?.message || error.response?.data

      if (serverMessage && typeof serverMessage === 'string') {
        return serverMessage
      }

      switch (status) {
        case 404:
          return 'Неправильный логин или пароль'
        case 401:
          return 'Неправильный логин или пароль'
        case 409:
          return 'Пользователь с таким именем уже существует'
        case 500:
          return 'Внутренняя ошибка сервера'
        case 503:
          return 'Сервис временно недоступен'
        default:
          return error.message || 'Произошла неизвестная ошибка'
      }
    }
    return 'Произошла неизвестная ошибка'
  }

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target
    setFormData(prev => ({
      ...prev,
      [name]: value
    }))
    
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }))
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErrors({})
    setSubmitError('')
    setSuccessMessage('')

    try {
      if (isLogin) {
        const loginData: LoginFormData = {
          username: formData.username,
          password: formData.password
        }
        loginSchema.parse(loginData)
        setIsSubmitting(true)
        await login(loginData.username, loginData.password)
        setSuccessMessage('Вход выполнен успешно! Перенаправление...')
      } else {
        const registerData: RegisterFormData = {
          username: formData.username,
          email: formData.email,
          password: formData.password
        }
        registerSchema.parse(registerData)
        setIsSubmitting(true)
        await register(registerData.username, registerData.email, registerData.password)
        setSuccessMessage('Регистрация прошла успешно! Перенаправление...')
      }
    } catch (error) {
      if (error instanceof z.ZodError) {
        const fieldErrors: Record<string, string> = {}
        error.errors.forEach(err => {
          if (err.path[0]) {
            fieldErrors[err.path[0] as string] = err.message
          }
        })
        setErrors(fieldErrors)
      } else {
        setSubmitError(getErrorMessage(error))
      }
    } finally {
      setIsSubmitting(false)
    }
  }

  const switchTab = (loginMode: boolean) => {
    setIsLogin(loginMode)
    setFormData({ username: '', email: '', password: '' })
    setErrors({})
    setSubmitError('')
    setSuccessMessage('')
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-50 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-gradient-to-r from-blue-600 to-indigo-600 rounded-2xl mb-4 shadow-lg">
            <UserIcon className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-slate-900 mb-2">
            Финансовый менеджер
          </h1>
        </div>

        {/* Main Card */}
        <Card className="shadow-xl border-0">
          <CardContent className="pt-6">
            <Tabs value={isLogin ? "login" : "register"} onValueChange={(value) => switchTab(value === "login")} className="w-full">
              <TabsList className="grid w-full grid-cols-2 mb-6">
                <TabsTrigger value="login">Вход</TabsTrigger>
                <TabsTrigger value="register">Регистрация</TabsTrigger>
              </TabsList>

              {/* Success Message */}
              {successMessage && (
                <Alert className="mb-6 border-green-200 bg-green-50">
                  <CheckCircle2 className="h-4 w-4 text-green-600" />
                  <AlertDescription className="text-green-800 font-medium">
                    {successMessage}
                  </AlertDescription>
                </Alert>
              )}

              {/* Error Message */}
              {submitError && (
                <Alert variant="destructive" className="mb-6">
                  <AlertCircle className="h-4 w-4" />
                  <AlertDescription className="font-medium">
                    {submitError}
                  </AlertDescription>
                </Alert>
              )}

              <TabsContent value="login" className="mt-0">
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div className="space-y-2">
                    <Input
                      id="username"
                      name="username"
                      type="text"
                      value={formData.username}
                      onChange={handleInputChange}
                      disabled={isSubmitting}
                      placeholder="Имя пользователя"
                      className={`h-11 ${errors.username ? "border-red-500 focus-visible:ring-red-500" : ""}`}
                    />
                    {errors.username && (
                      <p className="text-sm text-red-600 font-medium">{errors.username}</p>
                    )}
                  </div>

                  <div className="space-y-2">
                    <Input
                      id="password"
                      name="password"
                      type="password"
                      value={formData.password}
                      onChange={handleInputChange}
                      disabled={isSubmitting}
                      placeholder="Пароль"
                      className={`h-11 ${errors.password ? "border-red-500 focus-visible:ring-red-500" : ""}`}
                    />
                    {errors.password && (
                      <p className="text-sm text-red-600 font-medium">{errors.password}</p>
                    )}
                  </div>

                  <Button
                    type="submit"
                    disabled={isSubmitting}
                    className="w-full h-11 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700"
                  >
                    {isSubmitting ? (
                      <>
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        Вход...
                      </>
                    ) : (
                      'Войти'
                    )}
                  </Button>
                </form>
              </TabsContent>

              <TabsContent value="register" className="mt-0">
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div className="space-y-2">
                    <Input
                      id="username-register"
                      name="username"
                      type="text"
                      value={formData.username}
                      onChange={handleInputChange}
                      disabled={isSubmitting}
                      placeholder="Имя пользователя"
                      className={`h-11 ${errors.username ? "border-red-500 focus-visible:ring-red-500" : ""}`}
                    />
                    {errors.username && (
                      <p className="text-sm text-red-600 font-medium">{errors.username}</p>
                    )}
                  </div>

                  <div className="space-y-2">
                    <Input
                      id="email"
                      name="email"
                      type="email"
                      value={formData.email}
                      onChange={handleInputChange}
                      disabled={isSubmitting}
                      placeholder="Email"
                      className={`h-11 ${errors.email ? "border-red-500 focus-visible:ring-red-500" : ""}`}
                    />
                    {errors.email && (
                      <p className="text-sm text-red-600 font-medium">{errors.email}</p>
                    )}
                  </div>

                  <div className="space-y-2">
                    <Input
                      id="password-register"
                      name="password"
                      type="password"
                      value={formData.password}
                      onChange={handleInputChange}
                      disabled={isSubmitting}
                      placeholder="Пароль"
                      className={`h-11 ${errors.password ? "border-red-500 focus-visible:ring-red-500" : ""}`}
                    />
                    {errors.password && (
                      <p className="text-sm text-red-600 font-medium">{errors.password}</p>
                    )}
                  </div>

                  <Button
                    type="submit"
                    disabled={isSubmitting}
                    className="w-full h-11 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700"
                  >
                    {isSubmitting ? (
                      <>
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        Регистрация...
                      </>
                    ) : (
                      'Зарегистрироваться'
                    )}
                  </Button>
                </form>
              </TabsContent>
            </Tabs>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

export default AuthPage