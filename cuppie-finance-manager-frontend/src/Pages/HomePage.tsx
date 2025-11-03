import React, { useState, useEffect, useMemo } from "react"
import { useAuth } from "../context/AuthContext"
import {
  LogOut,
  Plus,
  Edit,
  Trash2,
  TrendingUp,
  TrendingDown,
  Wallet,
  Tag,
  User,
  AlertCircle,
  MoreVertical,
  X
} from "lucide-react"
import {
  PieChart as RechartsPie,
  Pie,
  Cell,
  Tooltip,
  Legend,
  ResponsiveContainer
} from "recharts"
import TransactionFilter from "../components/TransactionFilter"
import { DateTimePicker } from "@/components/ui/date-time-picker"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent } from "@/components/ui/card"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Skeleton } from "@/components/ui/skeleton"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"

const API_BASE_URL = "http://localhost:5295/api"

interface Category {
  id: number
  name: string
  income?: boolean
}

interface Transaction {
  id: number
  amount: number
  income: boolean
  categoryId: number
  categoryName?: string
  comment?: string
  createdAt: string
  userId: number
}

interface TransactionFilters {
  type?: "all" | "income" | "expense"
  categories?: number[]
  fromDate?: Date
  toDate?: Date
}

const COLORS = [
  "#6366F1", "#22C55E", "#EF4444", "#F59E0B", "#3B82F6",
  "#A855F7", "#EC4899", "#14B8A6", "#8B5CF6", "#84CC16"
]

const formatNumber = (num: number): string => {
  const isNegative = num < 0
  const absNum = Math.abs(num)
  const parts = absNum.toFixed(2).split('.')
  parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, " ")
  return (isNegative ? '-' : '') + parts.join('.')
}

const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (days === 0) {
    return `Сегодня, ${date.toLocaleTimeString("ru-RU", { hour: '2-digit', minute: '2-digit' })}`
  } else if (days === 1) {
    return `Вчера, ${date.toLocaleTimeString("ru-RU", { hour: '2-digit', minute: '2-digit' })}`
  } else if (days < 7) {
    return date.toLocaleDateString("ru-RU", { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })
  }
  return date.toLocaleDateString("ru-RU", { day: 'numeric', month: 'short', year: 'numeric' })
}

const HomePage: React.FC = () => {
  const { user, logout } = useAuth()

  const [categories, setCategories] = useState<Category[]>([])
  const [transactions, setTransactions] = useState<Transaction[]>([])
  const [viewMode, setViewMode] = useState<"table" | "chart">("table")
  const [showDeleteModal, setShowDeleteModal] = useState<number | null>(null)
  const [filters, setFilters] = useState<TransactionFilters>(() => {
    // По умолчанию устанавливаем фильтр "Этот месяц"
    const now = new Date()
    const firstDay = new Date(now.getFullYear(), now.getMonth(), 1)
    const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0)
    return {
      fromDate: firstDay,
      toDate: lastDay
    }
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  
  // Модальные окна
  const [showAddModal, setShowAddModal] = useState(false)
  const [showEditModal, setShowEditModal] = useState(false)
  const [showCategoriesModal, setShowCategoriesModal] = useState(false)
  
  // Формы
  const [transactionForm, setTransactionForm] = useState({
    amount: "",
    income: false,
    categoryId: "1",
    comment: "",
    createdAt: ""
  })
  const [editingTransaction, setEditingTransaction] = useState<Transaction | null>(null)
  const [editingCategory, setEditingCategory] = useState<Category | null>(null)
  const [newCategory, setNewCategory] = useState({ name: "", income: false })
  const [showDeleteCategoryModal, setShowDeleteCategoryModal] = useState<number | null>(null)

  const loadCategories = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/categories/all`, {
        credentials: 'include'
      })
      if (!response.ok) throw new Error("Ошибка загрузки категорий")
      const data = await response.json()
      setCategories(data)
    } catch (err) {
      console.error("Ошибка загрузки категорий:", err)
      setError("Не удалось загрузить категории")
    }
  }

  const loadTransactions = async () => {
    if (!user?.id) return
    
    setLoading(true)
    setError(null)
    
    try {
      const filterDto: any = { userId: user.id }

      if (filters.type === "income") filterDto.income = true
      else if (filters.type === "expense") filterDto.income = false
      if (filters.categories && filters.categories.length > 0) filterDto.categoryIdList = filters.categories
      if (filters.fromDate) filterDto.from = filters.fromDate.toISOString()
      if (filters.toDate) filterDto.to = filters.toDate.toISOString()

      const response = await fetch(`${API_BASE_URL}/transactions/get`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(filterDto)
      })

      if (!response.ok) throw new Error("Ошибка загрузки транзакций")
      const data = await response.json()
      setTransactions(data)
    } catch (err) {
      console.error("Ошибка загрузки транзакций:", err)
      setError("Не удалось загрузить транзакции")
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    if (user?.id) {
      loadCategories()
      loadTransactions()
    }
  }, [user?.id, filters])

  const handleAddTransaction = async () => {
    if (!transactionForm.amount || !transactionForm.categoryId || !user?.id) return

    try {
      const createDto = {
        amount: parseFloat(transactionForm.amount),
        income: transactionForm.income,
        categoryId: parseInt(transactionForm.categoryId),
        comment: transactionForm.comment || null,
        userId: user.id,
        createdAt: transactionForm.createdAt ? new Date(transactionForm.createdAt).toISOString() : new Date().toISOString()
      }

      const response = await fetch(`${API_BASE_URL}/transactions`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(createDto)
      })

      if (!response.ok) throw new Error("Ошибка создания транзакции")

      setTransactionForm({ amount: "", income: false, categoryId: "1", comment: "", createdAt: "" })
      setShowAddModal(false)
      await loadTransactions()
    } catch (err: any) {
      console.error("Ошибка создания транзакции:", err)
      setError(err.message || "Не удалось создать транзакцию")
    }
  }

  const handleUpdateTransaction = async () => {
    if (!editingTransaction) return

    try {
      const response = await fetch(`${API_BASE_URL}/transactions`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(editingTransaction)
      })

      if (!response.ok) throw new Error("Ошибка обновления")

      setEditingTransaction(null)
      setShowEditModal(false)
      await loadTransactions()
    } catch (err: any) {
      console.error("Ошибка обновления транзакции:", err)
      setError(err.message || "Не удалось обновить транзакцию")
    }
  }

  const handleDelete = async () => {
    if (showDeleteModal == null) return

    try {
      const response = await fetch(`${API_BASE_URL}/transactions?id=${showDeleteModal}`, {
        method: 'DELETE',
        credentials: 'include'
      })

      if (!response.ok) throw new Error("Ошибка удаления")

      setShowDeleteModal(null)
      await loadTransactions()
    } catch (err: any) {
      console.error("Ошибка удаления транзакции:", err)
      setError(err.message || "Не удалось удалить транзакцию")
      setShowDeleteModal(null)
    }
  }

  const handleCreateCategory = async () => {
    if (!newCategory.name.trim()) return

    try {
      const response = await fetch(`${API_BASE_URL}/categories`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({
          name: newCategory.name.trim(),
          income: newCategory.income
        })
      })

      if (!response.ok) throw new Error("Ошибка создания категории")

      setNewCategory({ name: "", income: false })
      await loadCategories()
    } catch (err: any) {
      console.error("Ошибка создания категории:", err)
      setError(err.message || "Не удалось создать категорию")
    }
  }

  const handleUpdateCategory = async () => {
    if (!editingCategory || !editingCategory.name.trim()) return

    try {
      const response = await fetch(`${API_BASE_URL}/categories`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({
          id: editingCategory.id,
          name: editingCategory.name.trim(),
          income: editingCategory.income
        })
      })

      if (!response.ok) throw new Error("Ошибка обновления категории")

      setEditingCategory(null)
      await loadCategories()
    } catch (err: any) {
      console.error("Ошибка обновления категории:", err)
      setError(err.message || "Не удалось обновить категорию")
    }
  }

  const handleDeleteCategory = async () => {
    if (showDeleteCategoryModal === null) return
    if (showDeleteCategoryModal === 1) {
      setError("Эту категорию нельзя удалить")
      setShowDeleteCategoryModal(null)
      return
    }

    try {
      const response = await fetch(`${API_BASE_URL}/categories?id=${showDeleteCategoryModal}`, {
        method: 'DELETE',
        credentials: 'include'
      })

      if (!response.ok) throw new Error("Ошибка удаления")

      setShowDeleteCategoryModal(null)
      await loadCategories()
      await loadTransactions()
      
      if (filters.categories?.includes(showDeleteCategoryModal)) {
        setFilters({
          ...filters,
          categories: filters.categories.filter(id => id !== showDeleteCategoryModal)
        })
      }
    } catch (err: any) {
      console.error("Ошибка удаления категории:", err)
      setError(err.message || "Не удалось удалить категорию")
      setShowDeleteCategoryModal(null)
    }
  }

  const handleLogout = async () => {
    try {
      await logout()
    } catch (err) {
      console.error("Ошибка при выходе:", err)
    }
  }

  const getFilteredCategories = (isIncome: boolean) => {
    return categories.filter(cat => cat.income === isIncome)
  }

  const getActivePeriodFilter = (): string | null => {
    if (!filters.fromDate || !filters.toDate) return null
    
    const now = new Date()
    const from = new Date(filters.fromDate)
    const to = new Date(filters.toDate)
    
    // Проверка на "сегодня"
    const todayStart = new Date(now.getFullYear(), now.getMonth(), now.getDate())
    const todayEnd = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59, 999)
    if (from.getTime() === todayStart.getTime() && to.getTime() === todayEnd.getTime()) {
      return "Сегодня"
    }
    
    // Проверка на "этот месяц"
    const thisMonthFirst = new Date(now.getFullYear(), now.getMonth(), 1)
    const thisMonthLast = new Date(now.getFullYear(), now.getMonth() + 1, 0)
    if (from.getTime() === thisMonthFirst.getTime() && to.getTime() === thisMonthLast.getTime()) {
      return "Этот месяц"
    }
    
    // Проверка на "прошлый месяц"
    const lastMonthFirst = new Date(now.getFullYear(), now.getMonth() - 1, 1)
    const lastMonthLast = new Date(now.getFullYear(), now.getMonth(), 0)
    if (from.getTime() === lastMonthFirst.getTime() && to.getTime() === lastMonthLast.getTime()) {
      return "Прошлый месяц"
    }
    
    // Проверка на "этот год"
    const thisYearFirst = new Date(now.getFullYear(), 0, 1)
    const thisYearLast = new Date(now.getFullYear(), 11, 31)
    if (from.getTime() === thisYearFirst.getTime() && to.getTime() === thisYearLast.getTime()) {
      return "Этот год"
    }
    
    return "Период"
  }

  const handlePeriodFilter = (type: "today" | "thisMonth" | "lastMonth" | "thisYear") => {
    const now = new Date()
    let firstDay: Date, lastDay: Date
    
    switch(type) {
      case "today":
        firstDay = new Date(now.getFullYear(), now.getMonth(), now.getDate())
        lastDay = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59, 999)
        break
      case "thisMonth":
        firstDay = new Date(now.getFullYear(), now.getMonth(), 1)
        lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0)
        break
      case "lastMonth":
        firstDay = new Date(now.getFullYear(), now.getMonth() - 1, 1)
        lastDay = new Date(now.getFullYear(), now.getMonth(), 0)
        break
      case "thisYear":
        firstDay = new Date(now.getFullYear(), 0, 1)
        lastDay = new Date(now.getFullYear(), 11, 31)
        break
    }
    
    setFilters({ ...filters, fromDate: firstDay, toDate: lastDay })
  }

  const totalIncome = transactions.filter((t) => t.income).reduce((sum, t) => sum + t.amount, 0)
  const totalExpense = transactions.filter((t) => !t.income).reduce((sum, t) => sum + t.amount, 0)
  const balance = totalIncome - totalExpense

  const incomeChartData = useMemo(() => {
    const grouped: Record<number, number> = {}
    transactions.filter((t) => t.income).forEach((t) => {
      grouped[t.categoryId] = (grouped[t.categoryId] || 0) + t.amount
    })
    
    const total = Object.values(grouped).reduce((sum, val) => sum + val, 0)
    
    return Object.entries(grouped).map(([catId, amount]) => ({
      name: categories.find((c) => c.id === Number(catId))?.name || "Неизвестно",
      value: amount,
      percentage: total > 0 ? ((amount / total) * 100).toFixed(1) : "0"
    }))
  }, [transactions, categories])

  const expenseChartData = useMemo(() => {
    const grouped: Record<number, number> = {}
    transactions.filter((t) => !t.income).forEach((t) => {
      grouped[t.categoryId] = (grouped[t.categoryId] || 0) + t.amount
    })
    
    const total = Object.values(grouped).reduce((sum, val) => sum + val, 0)
    
    return Object.entries(grouped).map(([catId, amount]) => ({
      name: categories.find((c) => c.id === Number(catId))?.name || "Неизвестно",
      value: amount,
      percentage: total > 0 ? ((amount / total) * 100).toFixed(1) : "0"
    }))
  }, [transactions, categories])

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100">
      {/* Компактный Header */}
      <div className="bg-white border-b border-slate-200 sticky top-0 z-20 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 py-3 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <Avatar className="h-10 w-10 border-2 border-indigo-200">
              <AvatarFallback className="bg-gradient-to-br from-indigo-500 to-blue-500 text-white font-bold">
                {user?.username?.[0]?.toUpperCase() || "U"}
              </AvatarFallback>
            </Avatar>
          <div>
              <p className="text-sm font-semibold text-slate-900">Привет, {user?.username}!</p>
              <p className="text-xs text-slate-500">Финансовый менеджер</p>
          </div>
        </div>

          <div className="flex items-center gap-2">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setShowCategoriesModal(true)}
              className="hidden sm:flex"
            >
              <Tag className="w-4 h-4 mr-2" />
              Категории
            </Button>
            
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="h-10 w-10">
                  <User className="w-5 h-5" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-56">
                <DropdownMenuLabel>
                  <div className="flex flex-col space-y-1">
                    <p className="text-sm font-medium">{user?.username}</p>
                    <p className="text-xs text-muted-foreground">{user?.email}</p>
            </div>
                </DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem 
                  onClick={() => setShowCategoriesModal(true)}
                  className="sm:hidden"
                >
                  <Tag className="mr-2 h-4 w-4" />
                  Категории
                </DropdownMenuItem>
                <DropdownMenuItem onClick={handleLogout} className="text-red-600">
                  <LogOut className="mr-2 h-4 w-4" />
                  Выйти
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
            </div>
            </div>
            </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 py-6">
        {/* Ошибки */}
        {error && (
          <Alert variant="destructive" className="mb-4">
            <AlertCircle className="h-4 w-4" />
            <AlertTitle>Ошибка</AlertTitle>
            <AlertDescription className="flex items-center justify-between">
              <span>{error}</span>
              <Button onClick={() => setError(null)} variant="ghost" size="icon" className="h-6 w-6">
                <X className="h-4 w-4" />
              </Button>
            </AlertDescription>
          </Alert>
        )}

        {/* Sticky карточки итогов */}
        <div className="sticky top-[73px] z-10 bg-gradient-to-br from-slate-50 to-slate-100 pb-4 mb-4">
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            <Card className="border-2 border-green-200 bg-gradient-to-br from-green-50 to-emerald-50">
              <CardContent className="p-4">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className="p-2 bg-green-100 rounded-lg">
                <TrendingUp className="w-5 h-5 text-green-600" />
              </div>
                    <div>
                      <p className="text-xs text-green-600 font-medium">Доход</p>
                      <p className="text-2xl font-bold text-green-700">{formatNumber(totalIncome)}</p>
            </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card className="border-2 border-red-200 bg-gradient-to-br from-red-50 to-rose-50">
              <CardContent className="p-4">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className="p-2 bg-red-100 rounded-lg">
                <TrendingDown className="w-5 h-5 text-red-600" />
              </div>
                    <div>
                      <p className="text-xs text-red-600 font-medium">Расход</p>
                      <p className="text-2xl font-bold text-red-700">{formatNumber(totalExpense)}</p>
            </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card className={`border-2 ${balance >= 0 ? 'border-blue-200 bg-gradient-to-br from-blue-50 to-indigo-50' : 'border-orange-200 bg-gradient-to-br from-orange-50 to-red-50'}`}>
              <CardContent className="p-4">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className={`p-2 rounded-lg ${balance >= 0 ? 'bg-blue-100' : 'bg-orange-100'}`}>
                <Wallet className={`w-5 h-5 ${balance >= 0 ? 'text-blue-600' : 'text-orange-600'}`} />
              </div>
                    <div>
                      <p className={`text-xs font-medium ${balance >= 0 ? 'text-blue-600' : 'text-orange-600'}`}>Баланс</p>
                      <p className={`text-2xl font-bold ${balance >= 0 ? 'text-blue-700' : 'text-orange-700'}`}>{formatNumber(balance)}</p>
            </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>

        {/* Tabs и фильтры */}
        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 mb-4 bg-white p-3 rounded-lg border border-slate-200">
          <Tabs value={viewMode} onValueChange={(value) => setViewMode(value as "table" | "chart")} className="w-full sm:w-auto">
            <TabsList className="grid w-full sm:w-auto grid-cols-2">
              <TabsTrigger value="table" className="text-sm">
                📊 Таблица
              </TabsTrigger>
              <TabsTrigger value="chart" className="text-sm">
                📈 Диаграмма
              </TabsTrigger>
            </TabsList>
          </Tabs>

          <div className="flex items-center gap-2 w-full sm:w-auto">
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="outline" size="sm" className="flex-1 sm:flex-initial">
                  ⏱ {getActivePeriodFilter() || "Период"}
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onClick={() => handlePeriodFilter("today")}>
                  Сегодня
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => handlePeriodFilter("thisMonth")}>
                  Этот месяц
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => handlePeriodFilter("lastMonth")}>
                  Прошлый месяц
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => handlePeriodFilter("thisYear")}>
                  Этот год
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={() => setFilters({ ...filters, fromDate: undefined, toDate: undefined })}>
                  Сбросить период
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>

            <div className="flex-1 sm:flex-initial">
              <TransactionFilter
                filters={filters}
                allCategories={categories}
                onChange={setFilters}
              />
            </div>
          </div>
        </div>

        {/* Контент */}
          {loading ? (
          <div className="space-y-3">
            {[...Array(5)].map((_, i) => (
              <Skeleton key={i} className="h-16 w-full" />
            ))}
              </div>
            ) : (
          <Tabs value={viewMode} className="w-full">
            <TabsContent value="table" className="mt-0">
              {transactions.length === 0 ? (
                <Card className="border-dashed">
                  <CardContent className="flex flex-col items-center justify-center py-16">
                    <div className="text-6xl mb-4">📭</div>
                    <p className="text-xl font-semibold text-slate-700 mb-2">Нет транзакций</p>
                    <p className="text-sm text-muted-foreground mb-6">Добавьте первую транзакцию</p>
                    <Button onClick={() => setShowAddModal(true)}>
                      <Plus className="w-4 h-4 mr-2" />
                      Добавить транзакцию
                    </Button>
                  </CardContent>
                </Card>
              ) : (
                <>
                  {/* Desktop таблица */}
                  <Card className="hidden md:block">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Категория</TableHead>
                          <TableHead className="text-right">Сумма</TableHead>
                          <TableHead>Комментарий</TableHead>
                          <TableHead>Дата</TableHead>
                          <TableHead className="w-[50px]"></TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                    {transactions.map((t) => {
                          const category = categories.find((c) => c.id === t.categoryId)?.name || t.categoryName || "—"
                      return (
                            <TableRow key={t.id}>
                              <TableCell>
                                <div className="flex items-center gap-2">
                                  <Badge variant={t.income ? "default" : "secondary"} className="w-6 h-6 p-0 flex items-center justify-center">
                                    {t.income ? '↑' : '↓'}
                                  </Badge>
                                  <span className="font-medium">{category}</span>
                                </div>
                              </TableCell>
                              <TableCell className={`text-right font-bold ${t.income ? 'text-green-600' : 'text-red-600'}`}>
                                {t.income ? '+' : '−'}{formatNumber(t.amount)}
                              </TableCell>
                              <TableCell className="text-muted-foreground text-sm max-w-[200px] truncate">
                                {t.comment || "—"}
                              </TableCell>
                              <TableCell className="text-sm text-muted-foreground">
                                {formatDate(t.createdAt)}
                              </TableCell>
                              <TableCell>
                                <DropdownMenu>
                                  <DropdownMenuTrigger asChild>
                                    <Button variant="ghost" size="icon" className="h-8 w-8">
                                      <MoreVertical className="h-4 w-4" />
                                    </Button>
                                  </DropdownMenuTrigger>
                                  <DropdownMenuContent align="end">
                                    <DropdownMenuItem onClick={() => {
                                      setEditingTransaction({ ...t })
                                      setShowEditModal(true)
                                    }}>
                                      <Edit className="mr-2 h-4 w-4" />
                                      Редактировать
                                    </DropdownMenuItem>
                                    <DropdownMenuSeparator />
                                    <DropdownMenuItem 
                                      onClick={() => setShowDeleteModal(t.id)}
                                      className="text-red-600"
                                    >
                                      <Trash2 className="mr-2 h-4 w-4" />
                                      Удалить
                                    </DropdownMenuItem>
                                  </DropdownMenuContent>
                                </DropdownMenu>
                              </TableCell>
                            </TableRow>
                          )
                        })}
                      </TableBody>
                    </Table>
                  </Card>

                  {/* Mobile карточки */}
                  <div className="md:hidden space-y-3">
                    {transactions.map((t) => {
                      const category = categories.find((c) => c.id === t.categoryId)?.name || t.categoryName || "—"
                      return (
                        <Card key={t.id}>
                          <CardContent className="p-4">
                            <div className="flex items-start justify-between">
                              <div className="flex-1">
                                <div className="flex items-center gap-2 mb-2">
                                  <Badge variant={t.income ? "default" : "secondary"}>
                                    {t.income ? '💰' : '💸'} {category}
                                  </Badge>
                                </div>
                                {t.comment && (
                                  <p className="text-sm text-muted-foreground mb-2">{t.comment}</p>
                                )}
                                <p className="text-xs text-muted-foreground">
                                  {formatDate(t.createdAt)}
                                </p>
                              </div>
                              <div className="flex items-start gap-2">
                                <div className="text-right">
                                  <p className={`text-xl font-bold ${t.income ? 'text-green-600' : 'text-red-600'}`}>
                                    {t.income ? '+' : '−'}{formatNumber(t.amount)}
                                  </p>
                                </div>
                                <DropdownMenu>
                                  <DropdownMenuTrigger asChild>
                                    <Button variant="ghost" size="icon" className="h-8 w-8">
                                      <MoreVertical className="h-4 w-4" />
                                    </Button>
                                  </DropdownMenuTrigger>
                                  <DropdownMenuContent align="end">
                                    <DropdownMenuItem onClick={() => {
                                      setEditingTransaction({ ...t })
                                      setShowEditModal(true)
                                    }}>
                                      <Edit className="mr-2 h-4 w-4" />
                                      Редактировать
                                    </DropdownMenuItem>
                                    <DropdownMenuSeparator />
                                    <DropdownMenuItem 
                                  onClick={() => setShowDeleteModal(t.id)}
                                      className="text-red-600"
                                    >
                                      <Trash2 className="mr-2 h-4 w-4" />
                                      Удалить
                                    </DropdownMenuItem>
                                  </DropdownMenuContent>
                                </DropdownMenu>
                              </div>
                            </div>
                          </CardContent>
                        </Card>
                      )
                    })}
              </div>
                </>
              )}
            </TabsContent>

            <TabsContent value="chart" className="mt-0">
              <Card>
                <CardContent className="pt-6">
                  {incomeChartData.length === 0 && expenseChartData.length === 0 ? (
                    <div className="flex flex-col items-center justify-center py-16">
                      <div className="text-6xl mb-4">📊</div>
                      <p className="text-xl font-semibold text-slate-700 mb-2">Нет данных для диаграммы</p>
                      <p className="text-sm text-muted-foreground">Добавьте транзакции для визуализации</p>
                    </div>
                  ) : (
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                      <div>
                        <h3 className="text-lg font-bold text-green-700 mb-4 text-center flex items-center justify-center gap-2">
                          <TrendingUp className="w-5 h-5" />
                          Доходы
                        </h3>
                        {incomeChartData.length === 0 ? (
                          <div className="text-center text-slate-500 py-12">
                            <p className="text-sm">Нет данных о доходах</p>
                </div>
              ) : (
                          <ResponsiveContainer width="100%" height={300}>
                  <RechartsPie>
                    <Pie
                                data={incomeChartData}
                      dataKey="value"
                      nameKey="name"
                      cx="50%"
                      cy="50%"
                                outerRadius={100}
                                label={({ percentage }) => `${percentage}%`}
                                isAnimationActive={false}
                              >
                                {incomeChartData.map((_, index) => (
                                  <Cell key={`income-${index}`} fill={COLORS[index % COLORS.length]} />
                                ))}
                              </Pie>
                              <Tooltip 
                                formatter={(value: any, name: any, props: any) => [
                                  `${formatNumber(Number(value))} (${props.payload.percentage}%)`,
                                  name
                                ]} 
                              />
                              <Legend />
                            </RechartsPie>
                          </ResponsiveContainer>
                        )}
                      </div>

                      <div>
                        <h3 className="text-lg font-bold text-red-700 mb-4 text-center flex items-center justify-center gap-2">
                          <TrendingDown className="w-5 h-5" />
                          Расходы
                        </h3>
                        {expenseChartData.length === 0 ? (
                          <div className="text-center text-slate-500 py-12">
                            <p className="text-sm">Нет данных о расходах</p>
                          </div>
                        ) : (
                          <ResponsiveContainer width="100%" height={300}>
                            <RechartsPie>
                              <Pie
                                data={expenseChartData}
                                dataKey="value"
                                nameKey="name"
                                cx="50%"
                                cy="50%"
                                outerRadius={100}
                                label={({ percentage }) => `${percentage}%`}
                                isAnimationActive={false}
                              >
                                {expenseChartData.map((_, index) => (
                                  <Cell key={`expense-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                              <Tooltip 
                                formatter={(value: any, name: any, props: any) => [
                                  `${formatNumber(Number(value))} (${props.payload.percentage}%)`,
                                  name
                                ]} 
                              />
                    <Legend />
                  </RechartsPie>
                </ResponsiveContainer>
              )}
                      </div>
            </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        )}
      </div>

      {/* FAB кнопка */}
      <Button
        onClick={() => setShowAddModal(true)}
        className="fixed bottom-6 right-6 h-14 w-14 rounded-full shadow-2xl bg-gradient-to-r from-indigo-600 to-blue-600 hover:from-indigo-700 hover:to-blue-700"
        size="icon"
      >
        <Plus className="h-6 w-6" />
      </Button>

      {/* Модальное окно добавления транзакции */}
      <Dialog open={showAddModal} onOpenChange={setShowAddModal}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>Новая транзакция</DialogTitle>
            <DialogDescription>
              Добавьте новую доходную или расходную транзакцию
            </DialogDescription>
          </DialogHeader>
          
          <div className="space-y-4 py-4">
            <div className="grid grid-cols-2 gap-3">
              <div>
                <Input
                  type="number"
                  placeholder="Сумма *"
                  value={transactionForm.amount}
                  onChange={(e) => setTransactionForm({ ...transactionForm, amount: e.target.value })}
                />
              </div>
              <div>
                <Select
                  value={transactionForm.income ? "income" : "expense"}
                  onValueChange={(value) => {
                    const isIncome = value === "income"
                    const filteredCategories = getFilteredCategories(isIncome)
                    // Выбираем первую доступную категорию нового типа
                    const firstCategory = filteredCategories[0]
                    // Если нет категорий нового типа, пытаемся найти категорию с id=1, иначе оставляем пустым
                    const newCategoryId = firstCategory 
                      ? firstCategory.id.toString() 
                      : (categories.find(c => c.id === 1 && c.income === isIncome) ? "1" : "")
                    setTransactionForm({ 
                      ...transactionForm, 
                      income: isIncome,
                      categoryId: newCategoryId
                    })
                  }}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="expense">💸 Расход</SelectItem>
                    <SelectItem value="income">💰 Доход</SelectItem>
                  </SelectContent>
                </Select>
        </div>
      </div>

            <div>
              <Select
                value={transactionForm.categoryId}
                onValueChange={(value) => {
                  const selectedCategory = categories.find(c => c.id === parseInt(value))
                  // Проверяем, что выбранная категория соответствует типу транзакции
                  if (selectedCategory && selectedCategory.income === transactionForm.income) {
                    setTransactionForm({ ...transactionForm, categoryId: value })
                  }
                }}
                disabled={getFilteredCategories(transactionForm.income).length === 0}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Категория *" />
                </SelectTrigger>
                <SelectContent>
                  {getFilteredCategories(transactionForm.income).map((cat) => (
                    <SelectItem key={cat.id} value={cat.id.toString()}>
                      {cat.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {getFilteredCategories(transactionForm.income).length === 0 && (
                <p className="text-sm text-muted-foreground mt-1">
                  Нет категорий для {transactionForm.income ? "доходов" : "расходов"}
                </p>
              )}
              </div>

              <div>
              <Input
                placeholder="Комментарий (опционально)"
                value={transactionForm.comment}
                onChange={(e) => setTransactionForm({ ...transactionForm, comment: e.target.value })}
              />
              </div>

            <div>
              <DateTimePicker
                selected={transactionForm.createdAt ? new Date(transactionForm.createdAt) : null}
                onChange={(date) => setTransactionForm({
                  ...transactionForm,
                  createdAt: date ? date.toISOString() : ""
                })}
                placeholderText="Дата (опционально)"
              />
            </div>
          </div>

          <DialogFooter>
            <Button variant="outline" onClick={() => setShowAddModal(false)}>
              Отмена
            </Button>
            <Button 
              onClick={handleAddTransaction}
              disabled={!transactionForm.amount || !transactionForm.categoryId}
            >
              Добавить
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Модальное окно редактирования транзакции */}
      <Dialog open={showEditModal} onOpenChange={setShowEditModal}>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle>Редактировать транзакцию</DialogTitle>
            <DialogDescription>
              Измените данные транзакции
            </DialogDescription>
          </DialogHeader>
          
          {editingTransaction && (
            <div className="space-y-4 py-4">
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <Input
                    type="number"
                    placeholder="Сумма"
                    value={editingTransaction.amount}
                    onChange={(e) => setEditingTransaction({ 
                      ...editingTransaction, 
                      amount: parseFloat(e.target.value) || 0 
                    })}
                  />
                </div>
                <div>
                  <Select
                    value={editingTransaction.income ? "income" : "expense"}
                    onValueChange={(value) => {
                      const isIncome = value === "income"
                      const matchingCategories = getFilteredCategories(isIncome)
                      // Если текущая категория не подходит для нового типа, выбираем первую доступную
                      const currentCategory = categories.find(c => c.id === editingTransaction.categoryId)
                      const newCategoryId = currentCategory && currentCategory.income === isIncome 
                        ? editingTransaction.categoryId 
                        : (matchingCategories.length > 0 ? matchingCategories[0].id : editingTransaction.categoryId)
                      setEditingTransaction({ 
                        ...editingTransaction, 
                        income: isIncome, 
                        categoryId: newCategoryId 
                      })
                    }}
                  >
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="expense">💸 Расход</SelectItem>
                      <SelectItem value="income">💰 Доход</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div>
                <Select
                  value={editingTransaction.categoryId.toString()}
                  onValueChange={(value) => {
                    const selectedCategory = categories.find(c => c.id === parseInt(value))
                    // Проверяем, что выбранная категория соответствует типу транзакции
                    if (selectedCategory && selectedCategory.income === editingTransaction.income) {
                      setEditingTransaction({ 
                        ...editingTransaction, 
                        categoryId: parseInt(value) 
                      })
                    }
                  }}
                  disabled={getFilteredCategories(editingTransaction.income).length === 0}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {getFilteredCategories(editingTransaction.income).map((cat) => (
                      <SelectItem key={cat.id} value={cat.id.toString()}>
                        {cat.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {getFilteredCategories(editingTransaction.income).length === 0 && (
                  <p className="text-sm text-muted-foreground mt-1">
                    Нет категорий для {editingTransaction.income ? "доходов" : "расходов"}
                  </p>
                )}
              </div>

              <div>
                <Input
                  placeholder="Комментарий"
                  value={editingTransaction.comment || ""}
                  onChange={(e) => setEditingTransaction({ 
                    ...editingTransaction, 
                    comment: e.target.value 
                  })}
                />
              </div>

              <div>
                <DateTimePicker
                  selected={editingTransaction.createdAt ? new Date(editingTransaction.createdAt) : new Date()}
                  onChange={(date) => setEditingTransaction({
                    ...editingTransaction,
                    createdAt: date ? date.toISOString() : new Date().toISOString()
                  })}
                  isClearable={false}
                />
              </div>
            </div>
          )}

          <DialogFooter>
            <Button variant="outline" onClick={() => setShowEditModal(false)}>
                Отмена
            </Button>
            <Button onClick={handleUpdateTransaction}>
              Сохранить
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Модальное окно управления категориями */}
      <Dialog open={showCategoriesModal} onOpenChange={setShowCategoriesModal}>
        <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Tag className="w-5 h-5" />
              Категории
            </DialogTitle>
          </DialogHeader>

          <div className="space-y-4 py-4">
            <div className="border-b pb-4">
              <div className="flex gap-3">
                <Input
                  placeholder="Новая категория"
                  value={newCategory.name}
                  onChange={(e) => setNewCategory({ ...newCategory, name: e.target.value })}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') handleCreateCategory()
                  }}
                  className="flex-1"
                />
                <Select
                  value={newCategory.income ? "income" : "expense"}
                  onValueChange={(value) => setNewCategory({ ...newCategory, income: value === "income" })}
                >
                  <SelectTrigger className="w-[140px]">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="expense">💸 Расход</SelectItem>
                    <SelectItem value="income">💰 Доход</SelectItem>
                  </SelectContent>
                </Select>
                <Button onClick={handleCreateCategory} disabled={!newCategory.name.trim()}>
                  <Plus className="w-4 h-4" />
                </Button>
            </div>
          </div>

            <div>
              {categories.length === 0 ? (
                <p className="text-muted-foreground text-sm text-center py-8">
                  Нет категорий
                </p>
              ) : (
                <div className="space-y-2">
                  {categories.map((category) => {
                    const isEditing = editingCategory?.id === category.id
                    return (
                      <div
                        key={category.id}
                        className="flex items-center gap-3 p-3 border rounded-lg hover:bg-accent/50 transition-colors"
                      >
                        {isEditing ? (
                          <>
                            <Input
                              value={editingCategory.name}
                              onChange={(e) => setEditingCategory({ ...editingCategory, name: e.target.value })}
                              className="flex-1"
                              onKeyDown={(e) => {
                                if (e.key === 'Enter') handleUpdateCategory()
                                else if (e.key === 'Escape') setEditingCategory(null)
                              }}
                            />
                            <Select
                              value={editingCategory.income ? "income" : "expense"}
                              onValueChange={(value) => setEditingCategory({ ...editingCategory, income: value === "income" })}
                            >
                              <SelectTrigger className="w-[140px]">
                                <SelectValue />
                              </SelectTrigger>
                              <SelectContent>
                                <SelectItem value="expense">💸 Расход</SelectItem>
                                <SelectItem value="income">💰 Доход</SelectItem>
                              </SelectContent>
                            </Select>
                            <Button onClick={handleUpdateCategory} size="icon" variant="ghost">
                              <Edit className="w-4 h-4 text-green-600" />
                            </Button>
                            <Button onClick={() => setEditingCategory(null)} size="icon" variant="ghost">
                              <X className="w-4 h-4" />
                            </Button>
                          </>
                        ) : (
                          <>
                            <div className="flex-1 flex items-center gap-2">
                              <Badge variant={category.income ? "default" : "secondary"}>
                                {category.income ? "💰" : "💸"}
                              </Badge>
                              <span className="font-medium">{category.name}</span>
        </div>
                            <Button
                              onClick={() => setEditingCategory({ ...category })}
                              size="icon"
                              variant="ghost"
                            >
                              <Edit className="w-4 h-4" />
                            </Button>
                            <Button
                              onClick={() => setShowDeleteCategoryModal(category.id)}
                              size="icon"
                              variant="ghost"
                              disabled={category.id === 1}
                              className="text-red-500 hover:text-red-700"
                            >
                              <Trash2 className="w-4 h-4" />
                            </Button>
                          </>
                        )}
                      </div>
                    )
                  })}
                </div>
              )}
            </div>
          </div>
        </DialogContent>
      </Dialog>

      {/* Подтверждение удаления транзакции */}
      <AlertDialog open={showDeleteModal !== null} onOpenChange={(open) => !open && setShowDeleteModal(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Удалить транзакцию?</AlertDialogTitle>
            <AlertDialogDescription>
              Это действие нельзя отменить.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Отмена</AlertDialogCancel>
            <AlertDialogAction onClick={handleDelete} className="bg-red-600 hover:bg-red-700">
              Удалить
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Подтверждение удаления категории */}
      <AlertDialog open={showDeleteCategoryModal !== null} onOpenChange={(open) => !open && setShowDeleteCategoryModal(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Удалить категорию?</AlertDialogTitle>
            <AlertDialogDescription>
              Транзакции с этой категорией сохранятся.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Отмена</AlertDialogCancel>
            <AlertDialogAction onClick={handleDeleteCategory} className="bg-red-600 hover:bg-red-700">
              Удалить
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}

export default HomePage