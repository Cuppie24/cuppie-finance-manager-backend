import React, { useState, useEffect, useMemo } from "react"
import { useAuth } from "../context/AuthContext"
import {
  LogOut,
  Plus,
  Edit,
  Trash2,
  List,
  PieChart,
  TrendingUp,
  TrendingDown,
  Wallet,
  Save,
  X,
  Tag,
  User,
  AlertCircle
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
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Separator } from "@/components/ui/separator"
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
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
import { Calendar } from "@/components/ui/calendar"
import { Calendar as CalendarIcon } from "lucide-react"
import { format } from "date-fns"
import { ru } from "date-fns/locale"
import { cn } from "@/lib/utils"

// API Configuration
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

// Функция форматирования числа с разделителями тысяч
const formatNumber = (num: number): string => {
  const isNegative = num < 0
  const absNum = Math.abs(num)
  const parts = absNum.toFixed(2).split('.')
  parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, " ")
  return (isNegative ? '-' : '') + parts.join('.')
}

const HomePage: React.FC = () => {
  const { user, logout } = useAuth()

  const [categories, setCategories] = useState<Category[]>([])
  const [transactions, setTransactions] = useState<Transaction[]>([])
  const [editing, setEditing] = useState<Transaction | null>(null)
  const [viewMode, setViewMode] = useState<"table" | "chart">("table")
  const [showDeleteModal, setShowDeleteModal] = useState<number | null>(null)
  const [filters, setFilters] = useState<TransactionFilters>({})
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [newTransaction, setNewTransaction] = useState({
    amount: "",
    income: false,
    categoryId: "",
    comment: "",
    createdAt: ""
  })
  const [newTransactionDateOpen, setNewTransactionDateOpen] = useState(false)
  const [showCategoriesModal, setShowCategoriesModal] = useState(false)
  const [editingCategory, setEditingCategory] = useState<Category | null>(null)
  const [newCategory, setNewCategory] = useState({ name: "", income: false })
  const [showDeleteCategoryModal, setShowDeleteCategoryModal] = useState<number | null>(null)
  const [editingDateOpen, setEditingDateOpen] = useState(false)

  // Загрузка всех категорий
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

  // Загрузка транзакций с фильтрами для текущего пользователя
  const loadTransactions = async () => {
    if (!user?.id) return
    
    setLoading(true)
    setError(null)
    
    try {
      const filterDto: any = {
        userId: user.id // ВАЖНО: всегда фильтруем по текущему пользователю
      }

      if (filters.type === "income") {
        filterDto.income = true
      } else if (filters.type === "expense") {
        filterDto.income = false
      }

      if (filters.categories && filters.categories.length > 0) {
        filterDto.categoryIdList = filters.categories
      }

      if (filters.fromDate) {
        filterDto.from = filters.fromDate.toISOString()
      }

      if (filters.toDate) {
        filterDto.to = filters.toDate.toISOString()
      }

      const response = await fetch(`${API_BASE_URL}/transactions/get`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
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

  // Первоначальная загрузка
  useEffect(() => {
    if (user?.id) {
      loadCategories()
      loadTransactions()
    }
  }, [user?.id])

  // Перезагрузка при изменении фильтров
  useEffect(() => {
    if (user?.id) {
      loadTransactions()
    }
  }, [filters])

  const handleAddTransaction = async () => {
    if (!newTransaction.amount || !newTransaction.categoryId || !user?.id) return

    try {
      const createDto = {
        amount: parseFloat(newTransaction.amount),
        income: newTransaction.income,
        categoryId: parseInt(newTransaction.categoryId),
        comment: newTransaction.comment || null,
        userId: user.id, // ID текущего пользователя
        createdAt: newTransaction.createdAt ? new Date(newTransaction.createdAt).toISOString() : new Date().toISOString()
      }

      const response = await fetch(`${API_BASE_URL}/transactions`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify(createDto)
      })

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(`Ошибка создания транзакции: ${errorText}`)
      }

      setNewTransaction({ amount: "", income: false, categoryId: "", comment: "", createdAt: "" })
      await loadTransactions()
    } catch (err: any) {
      console.error("Ошибка создания транзакции:", err)
      setError(err.message || "Не удалось создать транзакцию")
    }
  }

  const handleDelete = async () => {
    if (showDeleteModal == null) return

    try {
      const response = await fetch(`${API_BASE_URL}/transactions?id=${showDeleteModal}`, {
        method: 'DELETE',
        credentials: 'include'
      })

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(`Ошибка удаления: ${errorText}`)
      }

      setShowDeleteModal(null)
      await loadTransactions()
    } catch (err: any) {
      console.error("Ошибка удаления транзакции:", err)
      setError(err.message || "Не удалось удалить транзакцию")
      setShowDeleteModal(null)
    }
  }

  const handleSave = async () => {
    if (!editing || !editing.amount || !editing.categoryId) return

    try {
      const patchDto = {
        id: editing.id,
        amount: editing.amount,
        income: editing.income,
        categoryId: editing.categoryId,
        comment: editing.comment || null,
        userId: editing.userId,
        createdAt: editing.createdAt
      }

      const response = await fetch(`${API_BASE_URL}/transactions`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify(patchDto)
      })

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(`Ошибка обновления: ${errorText}`)
      }

      setEditing(null)
      await loadTransactions()
    } catch (err: any) {
      console.error("Ошибка обновления транзакции:", err)
      setError(err.message || "Не удалось обновить транзакцию")
    }
  }

  const handleCancelEdit = () => {
    setEditing(null)
  }

  // Управление категориями
  const handleCreateCategory = async () => {
    if (!newCategory.name.trim()) return

    try {
      const response = await fetch(`${API_BASE_URL}/categories`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify({
          name: newCategory.name.trim(),
          income: newCategory.income
        })
      })

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(`Ошибка создания категории: ${errorText}`)
      }

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
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify({
          id: editingCategory.id,
          name: editingCategory.name.trim(),
          income: editingCategory.income
        })
      })

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(`Ошибка обновления категории: ${errorText}`)
      }

      setEditingCategory(null)
      await loadCategories()
    } catch (err: any) {
      console.error("Ошибка обновления категории:", err)
      setError(err.message || "Не удалось обновить категорию")
    }
  }

  const handleDeleteCategory = async () => {
    if (showDeleteCategoryModal === null) return

    // Запрещаем удаление категории с id === 1
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

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(`Ошибка удаления: ${errorText}`)
      }

      setShowDeleteCategoryModal(null)
      await loadCategories()
      await loadTransactions() // Обновляем таблицу транзакций
      // Очищаем фильтры, если удаленная категория была выбрана
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

  // Фильтрация категорий по типу транзакции (доход/расход)
  const getFilteredCategories = (isIncome: boolean) => {
    return categories.filter(cat => cat.income === isIncome)
  }

  // Определение активного фильтра периода
  const getActivePeriodFilter = (): "thisMonth" | "lastMonth" | "thisYear" | null => {
    if (!filters.fromDate || !filters.toDate) return null
    
    const now = new Date()
    const from = new Date(filters.fromDate)
    const to = new Date(filters.toDate)
    
    // Проверка на "этот месяц"
    const thisMonthFirst = new Date(now.getFullYear(), now.getMonth(), 1)
    const thisMonthLast = new Date(now.getFullYear(), now.getMonth() + 1, 0)
    if (from.getTime() === thisMonthFirst.getTime() && to.getTime() === thisMonthLast.getTime()) {
      return "thisMonth"
    }
    
    // Проверка на "прошлый месяц"
    const lastMonthFirst = new Date(now.getFullYear(), now.getMonth() - 1, 1)
    const lastMonthLast = new Date(now.getFullYear(), now.getMonth(), 0)
    if (from.getTime() === lastMonthFirst.getTime() && to.getTime() === lastMonthLast.getTime()) {
      return "lastMonth"
    }
    
    // Проверка на "этот год"
    const thisYearFirst = new Date(now.getFullYear(), 0, 1)
    const thisYearLast = new Date(now.getFullYear(), 11, 31)
    if (from.getTime() === thisYearFirst.getTime() && to.getTime() === thisYearLast.getTime()) {
      return "thisYear"
    }
    
    return null
  }

  const activePeriodFilter = getActivePeriodFilter()

  // Обработчики быстрой фильтрации
  const handleThisMonth = () => {
    const now = new Date()
    const firstDay = new Date(now.getFullYear(), now.getMonth(), 1)
    const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0)
    setFilters({
      ...filters,
      fromDate: firstDay,
      toDate: lastDay
    })
  }

  const handleLastMonth = () => {
    const now = new Date()
    const firstDay = new Date(now.getFullYear(), now.getMonth() - 1, 1)
    const lastDay = new Date(now.getFullYear(), now.getMonth(), 0)
    setFilters({
      ...filters,
      fromDate: firstDay,
      toDate: lastDay
    })
  }

  const handleThisYear = () => {
    const now = new Date()
    const firstDay = new Date(now.getFullYear(), 0, 1)
    const lastDay = new Date(now.getFullYear(), 11, 31)
    setFilters({
      ...filters,
      fromDate: firstDay,
      toDate: lastDay
    })
  }

  const totalIncome = transactions
    .filter((t) => t.income)
    .reduce((sum, t) => sum + t.amount, 0)
  const totalExpense = transactions
    .filter((t) => !t.income)
    .reduce((sum, t) => sum + t.amount, 0)
  const balance = totalIncome - totalExpense

  const incomeChartData = useMemo(() => {
    const grouped: Record<number, number> = {}
    transactions
      .filter((t) => t.income)
      .forEach((t) => {
        grouped[t.categoryId] = (grouped[t.categoryId] || 0) + t.amount
      })

    return Object.entries(grouped).map(([catId, total]) => ({
      name: categories.find((c) => c.id === Number(catId))?.name || transactions.find(t => t.categoryId === Number(catId))?.categoryName || "Неизвестно",
      value: total
    }))
  }, [transactions, categories])

  const expenseChartData = useMemo(() => {
    const grouped: Record<number, number> = {}
    transactions
      .filter((t) => !t.income)
      .forEach((t) => {
        grouped[t.categoryId] = (grouped[t.categoryId] || 0) + t.amount
      })

    return Object.entries(grouped).map(([catId, total]) => ({
      name: categories.find((c) => c.id === Number(catId))?.name || transactions.find(t => t.categoryId === Number(catId))?.categoryName || "Неизвестно",
      value: total
    }))
  }, [transactions, categories])

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-50 via-white to-blue-50 flex items-center justify-center p-4 sm:p-6">
      <div className="w-full max-w-7xl bg-white rounded-3xl shadow-2xl overflow-hidden border border-slate-200">
        {/* Header */}
        <div className="bg-gradient-to-r from-indigo-600 to-blue-600 text-white px-6 sm:px-8 py-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
          <div className="flex items-center gap-3">
            <Avatar className="h-12 w-12 border-2 border-white/20">
              <AvatarFallback className="bg-white/10 text-white text-lg font-bold">
                {user?.username?.[0]?.toUpperCase() || "U"}
              </AvatarFallback>
            </Avatar>
            <div>
              <h1 className="text-2xl sm:text-3xl font-bold">Привет, {user?.username}! 👋</h1>
              <p className="text-sm opacity-90 mt-1">Управляйте своими финансами эффективно</p>
            </div>
          </div>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="outline"
                className="flex items-center gap-2 bg-white/10 backdrop-blur-sm text-white border-white/20 hover:bg-white/20"
              >
                <User className="w-4 h-4" />
                <span className="font-medium">{user?.username}</span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuLabel>
                <div className="flex flex-col space-y-1">
                  <p className="text-sm font-medium leading-none">{user?.username}</p>
                  <p className="text-xs leading-none text-muted-foreground">
                    Финансовый менеджер
                  </p>
                </div>
              </DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={handleLogout} className="text-red-600 cursor-pointer">
                <LogOut className="mr-2 h-4 w-4" />
                <span>Выйти</span>
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>

        {/* Ошибки */}
        {error && (
          <div className="px-6 sm:px-8 py-4">
            <Alert variant="destructive">
              <AlertCircle className="h-4 w-4" />
              <AlertTitle>Ошибка</AlertTitle>
              <AlertDescription className="flex items-center justify-between">
                <span>{error}</span>
                <Button
                  onClick={() => setError(null)}
                  variant="ghost"
                  size="icon"
                  className="h-6 w-6"
                >
                  <X className="h-4 w-4" />
                </Button>
              </AlertDescription>
            </Alert>
          </div>
        )}

        {/* Добавление */}
        <div className="px-6 sm:px-8 py-6 bg-gradient-to-br from-slate-50 to-white border-b border-slate-200">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Plus className="w-5 h-5 text-indigo-600" />
                Новая транзакция
              </CardTitle>
              <CardDescription>Добавьте новую доходную или расходную транзакцию</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex flex-wrap gap-3 items-end">
            <div className="flex flex-col flex-1 min-w-[120px]">
              <Input
                type="number"
                value={newTransaction.amount}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => setNewTransaction({ ...newTransaction, amount: e.target.value })}
                placeholder="Сумма *"
              />
            </div>

            <div className="flex flex-col flex-1 min-w-[120px]">
              <Select
                value={newTransaction.income ? "income" : "expense"}
                onValueChange={(value: string) => {
                  const isIncome = value === "income"
                  // Reset categoryId when changing income type to avoid category mismatch
                  setNewTransaction({ ...newTransaction, income: isIncome, categoryId: "" })
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

            <div className="flex flex-col flex-1 min-w-[160px]">
              <Select
                value={newTransaction.categoryId}
                onValueChange={(value: string) => setNewTransaction({ ...newTransaction, categoryId: value })}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Категория *" />
                </SelectTrigger>
                <SelectContent>
                  {getFilteredCategories(newTransaction.income).map((cat) => (
                    <SelectItem key={cat.id} value={cat.id.toString()}>
                      {cat.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="flex flex-col flex-1 min-w-[200px]">
              <Input
                type="text"
                value={newTransaction.comment}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setNewTransaction({ ...newTransaction, comment: e.target.value })
                }
                placeholder="Комментарий (опционально)"
              />
            </div>

            <div className="flex flex-col flex-1 min-w-[200px]">
              <Popover open={newTransactionDateOpen} onOpenChange={setNewTransactionDateOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !newTransaction.createdAt && "text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {newTransaction.createdAt
                      ? format(new Date(newTransaction.createdAt), "dd.MM.yyyy HH:mm", { locale: ru })
                      : "Дата (опционально)"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={newTransaction.createdAt ? new Date(newTransaction.createdAt) : undefined}
                    onSelect={(date) => {
                      if (date) {
                        setNewTransaction({ ...newTransaction, createdAt: date.toISOString() })
                      } else {
                        setNewTransaction({ ...newTransaction, createdAt: "" })
                      }
                      setNewTransactionDateOpen(false)
                    }}
                    initialFocus
                    locale={ru}
                  />
                </PopoverContent>
              </Popover>
            </div>

                <Button
                  onClick={handleAddTransaction}
                  disabled={!newTransaction.amount || !newTransaction.categoryId}
                  className="flex items-center gap-2"
                >
                  <Plus className="w-5 h-5" /> Добавить
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Фильтр */}
        <div className="px-6 sm:px-8 py-6 bg-white border-b border-slate-200">
          <div className="flex justify-between items-center mb-3">
            <h3 className="text-lg font-bold text-slate-800 flex items-center gap-2">
              <span className="text-indigo-600">🔍</span> Фильтры
            </h3>
            <Button
              onClick={() => setShowCategoriesModal(true)}
              variant="outline"
              size="sm"
              className="flex items-center gap-2"
            >
              <Tag className="w-4 h-4" />
              Управление категориями
            </Button>
          </div>
          {/* Быстрые фильтры */}
          <div className="flex flex-wrap gap-2 mb-4">
            <Button
              onClick={handleThisMonth}
              variant={activePeriodFilter === "thisMonth" ? "default" : "secondary"}
              size="sm"
            >
              Этот месяц
            </Button>
            <Button
              onClick={handleLastMonth}
              variant={activePeriodFilter === "lastMonth" ? "default" : "secondary"}
              size="sm"
            >
              Прошлый месяц
            </Button>
            <Button
              onClick={handleThisYear}
              variant={activePeriodFilter === "thisYear" ? "default" : "secondary"}
              size="sm"
            >
              Этот год
            </Button>
          </div>
          <TransactionFilter
            filters={filters}
            allCategories={categories}
            onChange={setFilters}
          />
        </div>

        {/* Итоги */}
        <div className="px-6 sm:px-8 py-6 bg-gradient-to-br from-slate-50 to-white border-b border-slate-200">
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <Card className="bg-gradient-to-br from-green-50 to-emerald-50 border-2 border-green-200">
              <CardContent className="pt-6">
                <div className="flex items-center justify-between mb-2">
                  <p className="text-green-700 font-bold text-sm uppercase tracking-wide">Доход</p>
                  <TrendingUp className="w-5 h-5 text-green-600" />
                </div>
                <p className="text-green-700 text-3xl font-bold">{formatNumber(totalIncome)}</p>
              </CardContent>
            </Card>
            
            <Card className="bg-gradient-to-br from-red-50 to-rose-50 border-2 border-red-200">
              <CardContent className="pt-6">
                <div className="flex items-center justify-between mb-2">
                  <p className="text-red-700 font-bold text-sm uppercase tracking-wide">Расход</p>
                  <TrendingDown className="w-5 h-5 text-red-600" />
                </div>
                <p className="text-red-700 text-3xl font-bold">{formatNumber(totalExpense)}</p>
              </CardContent>
            </Card>
            
            <Card className={`bg-gradient-to-br ${balance >= 0 ? 'from-blue-50 to-indigo-50 border-blue-200' : 'from-orange-50 to-red-50 border-orange-200'} border-2`}>
              <CardContent className="pt-6">
                <div className="flex items-center justify-between mb-2">
                  <p className={`${balance >= 0 ? 'text-blue-700' : 'text-orange-700'} font-bold text-sm uppercase tracking-wide`}>Баланс</p>
                  <Wallet className={`w-5 h-5 ${balance >= 0 ? 'text-blue-600' : 'text-orange-600'}`} />
                </div>
                <p className={`${balance >= 0 ? 'text-blue-700' : 'text-orange-700'} text-3xl font-bold`}>{formatNumber(balance)}</p>
              </CardContent>
            </Card>
          </div>
        </div>

        <Separator />

        {/* Контент */}
        <div className="px-6 sm:px-8 py-6 bg-gradient-to-br from-slate-50 to-white">
          <Tabs value={viewMode} onValueChange={(value) => setViewMode(value as "table" | "chart")} className="w-full">
            <div className="flex justify-between items-center mb-6">
              <h3 className="text-lg font-bold text-slate-800">
                {viewMode === "table" ? "📊 Список транзакций" : "📈 Аналитика"}
              </h3>
              <TabsList>
                <TabsTrigger value="table" className="flex items-center gap-2">
                  <List className="w-4 h-4" /> Таблица
                </TabsTrigger>
                <TabsTrigger value="chart" className="flex items-center gap-2">
                  <PieChart className="w-4 h-4" /> Диаграмма
                </TabsTrigger>
              </TabsList>
            </div>

            {loading ? (
              <div className="space-y-4">
                <Skeleton className="h-12 w-full" />
                <Skeleton className="h-12 w-full" />
                <Skeleton className="h-12 w-full" />
                <Skeleton className="h-12 w-full" />
                <Skeleton className="h-12 w-full" />
              </div>
            ) : (
              <>
                <TabsContent value="table" className="mt-0">
                {transactions.length === 0 ? (
                  <Card className="border-dashed">
                    <CardContent className="flex flex-col items-center justify-center py-16">
                      <div className="text-5xl mb-3">📭</div>
                      <p className="text-lg font-medium text-muted-foreground">Нет транзакций</p>
                      <p className="text-sm mt-1 text-muted-foreground">Добавьте первую транзакцию выше</p>
                    </CardContent>
                  </Card>
                ) : (
                  <Card>
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Тип</TableHead>
                          <TableHead>Категория</TableHead>
                          <TableHead>Сумма</TableHead>
                          <TableHead>Комментарий</TableHead>
                          <TableHead className="min-w-[200px]">Дата</TableHead>
                          <TableHead className="text-center">Действия</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                    {transactions.map((t) => {
                      const category = categories.find((c) => c.id === t.categoryId)?.name || t.categoryName || "-"
                      const isEditing = editing?.id === t.id
                      
                      return (
                        <TableRow key={t.id} className={isEditing ? 'bg-indigo-50' : ''}>
                          <TableCell>
                            {isEditing ? (
                              <Select
                                value={editing.income ? "income" : "expense"}
                                onValueChange={(value: string) => {
                                  const isIncome = value === "income"
                                  // Reset categoryId to first matching category when changing income type
                                  const matchingCategories = getFilteredCategories(isIncome)
                                  const newCategoryId = matchingCategories.length > 0 ? matchingCategories[0].id : editing.categoryId
                                  setEditing({ ...editing, income: isIncome, categoryId: newCategoryId })
                                }}
                              >
                                <SelectTrigger className="w-full">
                                  <SelectValue />
                                </SelectTrigger>
                                <SelectContent>
                                  <SelectItem value="expense">💸 Расход</SelectItem>
                                  <SelectItem value="income">💰 Доход</SelectItem>
                                </SelectContent>
                              </Select>
                            ) : (
                              <Badge variant={t.income ? "default" : "destructive"} className="flex items-center gap-1.5 w-fit">
                                {t.income ? (
                                  <>
                                    <TrendingUp className="w-3.5 h-3.5" /> Доход
                                  </>
                                ) : (
                                  <>
                                    <TrendingDown className="w-3.5 h-3.5" /> Расход
                                  </>
                                )}
                              </Badge>
                            )}
                          </TableCell>
                          <TableCell>
                            {isEditing ? (
                              <Select
                                value={editing.categoryId.toString()}
                                onValueChange={(value: string) =>
                                  setEditing({ ...editing, categoryId: parseInt(value) })
                                }
                              >
                                <SelectTrigger className="w-full">
                                  <SelectValue />
                                </SelectTrigger>
                                <SelectContent>
                                  {getFilteredCategories(editing.income).map((cat) => (
                                    <SelectItem key={cat.id} value={cat.id.toString()}>
                                      {cat.name}
                                    </SelectItem>
                                  ))}
                                </SelectContent>
                              </Select>
                            ) : (
                              <span className="font-medium">{category}</span>
                            )}
                          </TableCell>
                          <TableCell>
                            {isEditing ? (
                              <Input
                                type="number"
                                value={editing.amount}
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                                  setEditing({ ...editing, amount: parseFloat(e.target.value) || 0 })
                                }
                                className="w-full"
                              />
                            ) : (
                              <span className="font-bold">{formatNumber(t.amount)}</span>
                            )}
                          </TableCell>
                          <TableCell>
                            {isEditing ? (
                              <Input
                                type="text"
                                value={editing.comment || ""}
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                                  setEditing({ ...editing, comment: e.target.value })
                                }
                                placeholder="Комментарий"
                                className="w-full"
                              />
                            ) : (
                              <span className="text-muted-foreground">{t.comment || "-"}</span>
                            )}
                          </TableCell>
                          <TableCell className="text-muted-foreground text-sm">
                            {isEditing ? (
                              <Popover open={editingDateOpen} onOpenChange={setEditingDateOpen}>
                                <PopoverTrigger asChild>
                                  <Button
                                    variant="outline"
                                    className={cn(
                                      "w-full justify-start text-left font-normal",
                                      !editing.createdAt && "text-muted-foreground"
                                    )}
                                  >
                                    <CalendarIcon className="mr-2 h-4 w-4" />
                                    {editing.createdAt
                                      ? format(new Date(editing.createdAt), "dd.MM.yyyy HH:mm", { locale: ru })
                                      : "Выберите дату"}
                                  </Button>
                                </PopoverTrigger>
                                <PopoverContent className="w-auto p-0" align="start">
                                  <Calendar
                                    mode="single"
                                    selected={editing.createdAt ? new Date(editing.createdAt) : undefined}
                                    onSelect={(date) => {
                                      if (date) {
                                        setEditing({ ...editing, createdAt: date.toISOString() })
                                      }
                                      setEditingDateOpen(false)
                                    }}
                                    initialFocus
                                    locale={ru}
                                  />
                                </PopoverContent>
                              </Popover>
                            ) : (
                              new Date(t.createdAt).toLocaleString("ru-RU")
                            )}
                          </TableCell>
                          <TableCell>
                            {isEditing ? (
                              <div className="flex gap-2 justify-center">
                                <Button
                                  onClick={handleSave}
                                  variant="ghost"
                                  size="icon"
                                  className="text-green-600 hover:text-green-800"
                                >
                                  <Save className="w-5 h-5" />
                                </Button>
                                <Button
                                  onClick={handleCancelEdit}
                                  variant="ghost"
                                  size="icon"
                                >
                                  <X className="w-5 h-5" />
                                </Button>
                              </div>
                            ) : (
                              <div className="flex gap-2 justify-center">
                                <Button
                                  onClick={() => setEditing({ ...t })}
                                  variant="ghost"
                                  size="icon"
                                  className="text-indigo-600 hover:text-indigo-800"
                                >
                                  <Edit className="w-5 h-5" />
                                </Button>
                                <Button
                                  onClick={() => setShowDeleteModal(t.id)}
                                  variant="ghost"
                                  size="icon"
                                  className="text-red-500 hover:text-red-700"
                                >
                                  <Trash2 className="w-5 h-5" />
                                </Button>
                              </div>
                            )}
                          </TableCell>
                        </TableRow>
                      )
                        })}
                      </TableBody>
                    </Table>
                  </Card>
                )}
              </TabsContent>

              <TabsContent value="chart" className="mt-0">
                <Card>
                  <CardContent className="pt-6">
                    {incomeChartData.length === 0 && expenseChartData.length === 0 ? (
                      <div className="flex flex-col items-center justify-center py-16">
                        <div className="text-5xl mb-3">📊</div>
                        <p className="text-lg font-medium text-muted-foreground">Нет данных для диаграммы</p>
                        <p className="text-sm mt-1 text-muted-foreground">Добавьте транзакции для визуализации</p>
                      </div>
                    ) : (
                      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                        {/* Диаграмма доходов */}
                        <div>
                          <h3 className="text-xl font-bold text-green-700 mb-4 text-center flex items-center justify-center gap-2">
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
                                  label
                                >
                                  {incomeChartData.map((_, index) => (
                                    <Cell
                                      key={`income-cell-${index}`}
                                      fill={COLORS[index % COLORS.length]}
                                    />
                                  ))}
                                </Pie>
                                <Tooltip formatter={(value: any) => formatNumber(Number(value))} />
                                <Legend />
                              </RechartsPie>
                            </ResponsiveContainer>
                          )}
                        </div>

                        {/* Диаграмма расходов */}
                        <div>
                          <h3 className="text-xl font-bold text-red-700 mb-4 text-center flex items-center justify-center gap-2">
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
                                  label
                                >
                                  {expenseChartData.map((_, index) => (
                                    <Cell
                                      key={`expense-cell-${index}`}
                                      fill={COLORS[index % COLORS.length]}
                                    />
                                  ))}
                                </Pie>
                                <Tooltip formatter={(value: any) => formatNumber(Number(value))} />
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
              </>
            )}
          </Tabs>
        </div>
      </div>

      {/* Модальное окно подтверждения удаления транзакции */}
      <AlertDialog open={showDeleteModal !== null} onOpenChange={(open: boolean) => !open && setShowDeleteModal(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <div className="flex items-center gap-3 mb-2">
              <div className="w-12 h-12 rounded-full bg-red-100 flex items-center justify-center">
                <Trash2 className="w-6 h-6 text-red-600" />
              </div>
              <div>
                <AlertDialogTitle>Удалить транзакцию?</AlertDialogTitle>
                <AlertDialogDescription>Это действие нельзя отменить</AlertDialogDescription>
              </div>
            </div>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => setShowDeleteModal(null)}>
              Отмена
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDelete}
              className="bg-red-600 hover:bg-red-700"
            >
              Удалить
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Модальное окно управления категориями */}
      <Dialog open={showCategoriesModal} onOpenChange={setShowCategoriesModal}>
        <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Tag className="w-5 h-5" />
              Управление категориями
            </DialogTitle>
            <DialogDescription>
              Добавьте, отредактируйте или удалите категории для ваших транзакций
            </DialogDescription>
          </DialogHeader>

          {/* Форма добавления новой категории */}
          <div className="space-y-4 py-4">
            <div className="border-b pb-4">
              <h3 className="font-semibold mb-3">Добавить новую категорию</h3>
              <div className="flex gap-3 items-end">
                <div className="flex-1">
                  <Input
                    placeholder="Название категории"
                    value={newCategory.name}
                    onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                      setNewCategory({ ...newCategory, name: e.target.value })
                    }
                    onKeyDown={(e: React.KeyboardEvent<HTMLInputElement>) => {
                      if (e.key === 'Enter') {
                        handleCreateCategory()
                      }
                    }}
                  />
                </div>
                <div className="w-[160px]">
                  <Select
                    value={newCategory.income ? "income" : "expense"}
                    onValueChange={(value: string) =>
                      setNewCategory({ ...newCategory, income: value === "income" })
                    }
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
                <Button onClick={handleCreateCategory} disabled={!newCategory.name.trim()}>
                  <Plus className="w-4 h-4 mr-2" />
                  Добавить
                </Button>
              </div>
            </div>

            {/* Список категорий */}
            <div>
              <h3 className="font-semibold mb-3">Существующие категории</h3>
              {categories.length === 0 ? (
                <p className="text-muted-foreground text-sm text-center py-8">
                  Нет категорий. Создайте первую категорию выше.
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
                              onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                                setEditingCategory({ ...editingCategory, name: e.target.value })
                              }
                              className="flex-1"
                              onKeyDown={(e: React.KeyboardEvent<HTMLInputElement>) => {
                                if (e.key === 'Enter') {
                                  handleUpdateCategory()
                                } else if (e.key === 'Escape') {
                                  setEditingCategory(null)
                                }
                              }}
                            />
                            <Select
                              value={editingCategory.income ? "income" : "expense"}
                              onValueChange={(value: string) =>
                                setEditingCategory({ ...editingCategory, income: value === "income" })
                              }
                            >
                              <SelectTrigger className="w-[140px]">
                                <SelectValue />
                              </SelectTrigger>
                              <SelectContent>
                                <SelectItem value="expense">💸 Расход</SelectItem>
                                <SelectItem value="income">💰 Доход</SelectItem>
                              </SelectContent>
                            </Select>
                            <Button
                              onClick={handleUpdateCategory}
                              size="icon"
                              variant="ghost"
                              className="text-green-600 hover:text-green-800"
                            >
                              <Save className="w-4 h-4" />
                            </Button>
                            <Button
                              onClick={() => setEditingCategory(null)}
                              size="icon"
                              variant="ghost"
                            >
                              <X className="w-4 h-4" />
                            </Button>
                          </>
                        ) : (
                          <>
                            <div className="flex-1 flex items-center gap-2">
                              <Badge variant={category.income ? "default" : "secondary"}>
                                {category.income ? "💰 Доход" : "💸 Расход"}
                              </Badge>
                              <span className="font-medium">{category.name}</span>
                            </div>
                            <Button
                              onClick={() => setEditingCategory({ ...category })}
                              size="icon"
                              variant="ghost"
                              className="text-indigo-600 hover:text-indigo-800"
                            >
                              <Edit className="w-4 h-4" />
                            </Button>
                            <Button
                              onClick={() => setShowDeleteCategoryModal(category.id)}
                              size="icon"
                              variant="ghost"
                              className="text-red-500 hover:text-red-700"
                              disabled={category.id === 1}
                              title={category.id === 1 ? "Эту категорию нельзя удалить" : "Удалить категорию"}
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

          <DialogFooter>
            <Button onClick={() => setShowCategoriesModal(false)} variant="outline">
              Закрыть
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Модальное окно подтверждения удаления категории */}
      <AlertDialog open={showDeleteCategoryModal !== null} onOpenChange={(open: boolean) => !open && setShowDeleteCategoryModal(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <div className="flex items-center gap-3 mb-2">
              <div className="w-12 h-12 rounded-full bg-red-100 flex items-center justify-center">
                <Trash2 className="w-6 h-6 text-red-600" />
              </div>
              <div>
                <AlertDialogTitle>Удалить категорию?</AlertDialogTitle>
                <AlertDialogDescription>
                  Это действие нельзя отменить. Все транзакции с этой категорией сохранятся, но категория будет удалена.
                </AlertDialogDescription>
              </div>
            </div>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => setShowDeleteCategoryModal(null)}>
              Отмена
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteCategory}
              className="bg-red-600 hover:bg-red-700"
            >
              Удалить
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  )
}

export default HomePage